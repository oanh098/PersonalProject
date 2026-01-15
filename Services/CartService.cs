using System;

namespace PersonalProject.Services;

public class CartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetOrSetCartId()
    {
        var session = _httpContextAccessor.HttpContext!.Session;

        if (session.IsAvailable && !session.TryGetValue("CartId", out _))// The out _ means: “Ignore the actual value — I only care if it exists or not.”
        {
            var cartId = Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);
            return cartId;
        }

        return session.GetString("CartId") ?? string.Empty;
    }

    

}
