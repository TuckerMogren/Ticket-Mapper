using MediatR;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WP = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using Color = SixLabors.ImageSharp.Color;
using Microsoft.Extensions.Logging;
using TicketMapper.Domain.DataModels;
using TicketMapper.Domain.Interfaces.Commands;

namespace TicketMapper.Application.Commands
{   
    public class CreateDocumentCommand(TicketDetails ticketDetails) : ICreateDocumentCommand
    {
        public TicketDetails TicketDetails { get; private set; } = ticketDetails ?? throw new ArgumentNullException(nameof(ticketDetails));

        public class CreateDocumentCommandHandler(ILogger<CreateDocumentCommandHandler> logger)
            : IRequestHandler<ICreateDocumentCommand, Unit>
        {
            private readonly ILogger<CreateDocumentCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            public async Task<Unit> Handle(ICreateDocumentCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    using Image<Rgba32> image = Image.Load<Rgba32>("Input Files/Ticket.png");
                    var font = SystemFonts.CreateFont("Arial", 30, FontStyle.Regular);

                    using WordprocessingDocument wordDocument = WordprocessingDocument.Create(request.TicketDetails.FileName, WordprocessingDocumentType.Document);
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    var sectionProps = new SectionProperties();
                    var pageMargin = new PageMargin()
                    {
                        Top = Int32Value.FromInt32(180),
                        Right = UInt32Value.FromUInt32(180),
                        Bottom = Int32Value.FromInt32(180),
                        Left = UInt32Value.FromUInt32(180)
                    };
                    sectionProps.Append(pageMargin);
                    body.Append(sectionProps);

                    for (var i = request.TicketDetails.StartNumber; i <= request.TicketDetails.EndNumber; i++)
                    {
                        var currentTicketNumber = i;
                        try
                        {
                            using var ticket = image.Clone();
                            ticket.Mutate(x => x.DrawText(currentTicketNumber.ToString(), font, Color.Black, new PointF(120, 290)));

                            string imagePath = $"Output Files/ticket_{i}.png";
                            await ticket.SaveAsync(imagePath, cancellationToken: cancellationToken);

                            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png);
                            using (FileStream stream = new FileStream(imagePath, FileMode.Open))
                            {
                                imagePart.FeedData(stream);
                            }

                            string imageId = mainPart.GetIdOfPart(imagePart);

                            // Calculate the size of the image to maintain aspect ratio
                            long imageWidthEmus = (long)image.Width * 9525; // Convert pixels to EMUs
                            long imageHeightEmus = (long)image.Height * 9525; // Convert pixels to EMUs

                            var paragraph = new Paragraph(
                                new Run(
                                    new Drawing(
                                        new WP.Inline(
                                            new WP.Extent() { Cx = imageWidthEmus, Cy = imageHeightEmus },
                                            new WP.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
                                            new WP.DocProperties() { Id = (UInt32Value)1U, Name = "Picture 1" },
                                            new WP.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
                                            new A.Graphic(
                                                new A.GraphicData(
                                                    new PIC.Picture(
                                                        new PIC.NonVisualPictureProperties(
                                                            new PIC.NonVisualDrawingProperties() { Id = (UInt32Value)0U, Name = "New Bitmap Image.jpg" },
                                                            new PIC.NonVisualPictureDrawingProperties()),
                                                        new PIC.BlipFill(
                                                            new A.Blip(new A.BlipExtensionList(new A.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" })) { Embed = imageId },
                                                            new A.Stretch(new A.FillRectangle())),
                                                        new PIC.ShapeProperties(
                                                            new A.Transform2D(new A.Offset() { X = 0L, Y = 0L },
                                                                new A.Extents() { Cx = imageWidthEmus, Cy = imageHeightEmus }),
                                                            new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle })
                                                    )
                                                )
                                                { Uri = "https://schemas.openxmlformats.org/drawingml/2006/picture" }
                                            )
                                        )
                                    )
                                )
                            );

                            body.AppendChild(paragraph);
                            
                            File.Delete(imagePath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error processing ticket {i}: {ex.Message}");
                        }
                    } 
                    _logger.LogInformation("Tickets created and added to Word document successfully!");
                }
                catch (OperationCanceledException e)
                {
                    _logger.LogInformation($"Operation was canceled {e.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error occurred: {ex.Message}");
                }

                return Unit.Value;
            }
        }
    }
}
