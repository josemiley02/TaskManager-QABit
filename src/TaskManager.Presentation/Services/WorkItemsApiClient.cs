using TaskManager.Presentation.Models.Common;
using TaskManager.Presentation.Models.WorkItems;

namespace TaskManager.Presentation.Services;

public class WorkItemsApiClient
{
    private readonly HttpClient _httpClient;

    public WorkItemsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResponse<WorkItemResponseList>> GetWorkItemsAsync(
        int page = 1,
        int pageSize = 10,
        string? status = null,
        string? assignee = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"Pages={page}&PerPages={pageSize}";
        if (!string.IsNullOrWhiteSpace(status))
        {
            query += $"&Status={Uri.EscapeDataString(status)}";
        }

        if (!string.IsNullOrWhiteSpace(assignee))
        {
            query += $"&Asignee={Uri.EscapeDataString(assignee.Trim())}";
        }

        using var response = await _httpClient.GetAsync($"api/WorkItems?{query}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<PagedResponse<WorkItemResponseList>>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("La API devolvió una respuesta vacía al listar las tareas.");
    }

    public async Task<WorkItemResponse> GetWorkItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"api/WorkItems/{id}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<WorkItemResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("La API devolvió una tarea vacía.");
    }

    public async Task<WorkItemResponse> CreateWorkItemAsync(
        WorkItemRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/WorkItems", request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<WorkItemResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("La API devolvió una tarea vacía después de crearla.");
    }

    public async Task<WorkItemResponse> UpdateWorkItemAsync(
        Guid id,
        WorkItemRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync($"api/WorkItems/{id}", request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<WorkItemResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("La API devolvió una tarea vacía después de actualizarla.");
    }

    public async Task DeleteWorkItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync($"api/WorkItems/{id}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var details = await response.Content.ReadAsStringAsync(cancellationToken);
        var message = string.IsNullOrWhiteSpace(details)
            ? $"La API respondió con {(int)response.StatusCode} ({response.ReasonPhrase})."
            : $"La API respondió con {(int)response.StatusCode}: {details}";

        throw new HttpRequestException(message, null, response.StatusCode);
    }
}
