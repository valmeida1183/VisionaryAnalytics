namespace WebApi.Endpoints.Video;

public static class VideoEndpoints 
{
    public static void AddVideoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("videos", async (IFormFile file, CancellationToken cancellationToken) =>
        {
            if (!FileExists(file))
            {
                return Results.BadRequest("File is empty.");
            }

            if (!isValidFileSize(file))
            {
                return Results.BadRequest("File size exceeds the limit of 100 MB.");
            }

            if (!IsValidFileExtension(file))
            {
                return Results.BadRequest("Invalid file extension. Only .mp4 or .avi allowed.");
            }

            if (!IsValidFileType(file))
            {
                return Results.BadRequest("Invalid file type. Only video files are allowed.");
            }

            // Simulate file upload logic
            await Task.Delay(1000, cancellationToken); // Simulating async operation

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
            return Results.Ok(new
            {
                Message = "File uploaded successfully.",
                FileName = fileName,
                OriginalName = file.FileName,
                Size = file.Length
            });
        })
        .Accepts<IFormFile>("multipart/form-data")
        .DisableAntiforgery()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .WithName("UploadVideo")
        .WithTags(EndpointTags.Video);
    }

    // TODO move this logic to a validator in Application layer
    private static bool FileExists(IFormFile file)
    {
        return file != null && file.Length > 0;
    }

    private static bool isValidFileSize(IFormFile file)
    {
        // Example: Check if file size is less than 100 MB
        const long maxFileSize = 100 * 1024 * 1024; // 100 MB
        return file.Length <= maxFileSize;
    }

    private static bool IsValidFileExtension(IFormFile file)
    {
        // Example: Check if the file type is allowed (e.g., video files)
        var allowedExtensions = new[] { ".mp4", ".avi" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(fileExtension);
    }

    private static bool IsValidFileType(IFormFile file)
    {
        // Example: Check if the file type is allowed (e.g., video files)
        var allowedMimeTypes = new[] { "video/mp4", "video/x-msvideo" };
        
        return allowedMimeTypes.Contains(file.ContentType.ToLower());
    }

    // Parei em Verificar result pattern, iniciar camada de Application e criar validações que estão aqui lá com FluentValidation.
    // Iniciar configuração da infraestrutura como banco de dados mongoDb, rabbitMq
}
