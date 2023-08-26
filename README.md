# Ticket Mapper

## Overview
Ticket Mapper is a .NET 7.0 application designed to manage and generate tickets. It utilizes MediatR for CQRS and is structured into three main projects:
- `TicketMapper.Application`: Handles commands and queries.
- `TicketMapper.Domain`: Contains data models and notifications.
- `TicketMapper.WebApi`: Exposes the functionality via RESTful API.

## Features

### Create Document
- **Command**: `CreateDocumentCommand`
- Generates a Word document with ticket details.
- Saves each ticket as an image and embeds it in the document.

### Delete Document
- **Command**: `DeleteDocumentCommand`
- Deletes a specified document from the file system.

### Time to Delete Notification
- **Notification**: `TimeToDeleteNotification`
- Notifies when it's time to delete a document.

### Get Document
- **Query**: `GetDocumentQuery`
- Reads a document from the file system and returns it as a byte array.

## Installation
1. Clone the repository
2. Build the solution
3. Run `TicketMapper.WebApi`

## API Endpoints
- `GET /api/v1/Document/DownloadDocument`: Downloads a document based on ticket details.
