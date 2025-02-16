using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversiteCsvProvider.Services;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;

namespace UniversiteAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Scolarite)]
public class NotesController : ControllerBase
{
    private readonly GenerateCsvService _generationService;
    private readonly UploadCsvService _uploadService;
    private readonly IUeRepository _ueRepository;

    public NotesController(
        GenerateCsvService generationService,
        UploadCsvService uploadService,
        IUeRepository ueRepository)
    {
        _generationService = generationService;
        _uploadService = uploadService;
        _ueRepository = ueRepository;
    }

    // Télécharger le template
    [HttpGet("template/{ueId}")]
    public IActionResult DownloadTemplate(long ueId)
    {
        var stream = _generationService.GenerateTemplate(ueId);
        return File(stream, "text/csv", $"Notes_UE_{ueId}.csv");
    }

    // Uploader les notes (avec ID de l'UE dans la route)
    [HttpPost("upload/{ueId}")]
    public async Task<IActionResult> UploadNotes(long ueId, IFormFile file)
    {
        // Récupération de l'UE cible
        var ue = await _ueRepository.FindAsync(ueId);
        if (ue == null)
        {
            return BadRequest("UE introuvable");
        }

        // Traitement du CSV
        var result = await _uploadService.ProcessUploadAsync(file.OpenReadStream(), ue);
        if (!result.Success)
        {
            return BadRequest(new { Errors = result.Errors });
        }

        return Ok("Notes mises à jour avec succès !");
    }
}