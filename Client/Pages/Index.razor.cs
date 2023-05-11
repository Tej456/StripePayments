using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Shared.Models;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Stripe;

namespace Client.Pages;

public partial class Index
{
    [Inject]
    public HttpClient HttpClient { get; set; } = default!;

    [Inject]
    public IJSRuntime JsRuntime { get; set; } = default!;

    private List<Book>? _products;
    private IEnumerable<Book[]>? _productChunksOf4;

    private const string DevApiBaseAddress = "https://localhost:7202";

    protected override async Task OnInitializedAsync()
    {
        _products = await HttpClient.GetFromJsonAsync<List<Book>>($"{DevApiBaseAddress}/books");

        if (_products is not null)
        {
            _productChunksOf4 = _products.Chunk(4);
        }
    }

    private async Task OnClickBtnBuyNowAsync(Book product)
    {
        var response = await HttpClient.PostAsJsonAsync($"{DevApiBaseAddress}/checkout", product);

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        var checkoutOrderResponse = JsonConvert.DeserializeObject<CheckoutOrderResponse>(responseBody);

        // Opens up Stripe.
        await JsRuntime.InvokeVoidAsync("checkout", checkoutOrderResponse.PubKey, checkoutOrderResponse.SessionId);
    }
}
