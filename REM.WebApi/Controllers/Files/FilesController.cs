// using REM.Application.Features.Files.Commands.UploadFile;

// namespace REM.WebApi.Controllers.Files;

// public class FilesController(ISender mediator) : BaseApiController
// {
//     private readonly ISender _mediator = mediator;

//     /// <summary>
//     /// Uploads multiple files to the specified folder.
//     /// </summary>
//     /// <param name="files">Collection of files to be uploaded.</param>
//     /// <param name="folderName">Folder name where the files will be saved.</param>
//     /// <returns>List of file paths.</returns>
//     [HttpPost("upload")]
//     public async Task<ActionResult<Result<List<string>>>> UploadFiles([FromForm] UploadFileCommand command)
//         => BaseResponseHandler(await _mediator.Send(command));
// }
