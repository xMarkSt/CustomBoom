# Boom API

## Encrypted request support

The API accepts both plain and AES-encrypted form submissions. Encrypted requests must supply the following values before the request body is decrypted:

| Field | Source | Description |
| ----- | ------ | ----------- |
| `_u`  | query or form | Player UUID used to identify the decrypting player. |
| `_p`  | query or form | Base64 encoded, AES-encrypted payload. |
| `_s`  | query or form | Base64 encoded initialization vector for decrypting the payload. |
| `_ct` | query or form | Original content type of the decrypted body (`application/x-www-form-urlencoded` or `multipart/form-data; boundary=...`). |

The `RequestDecryptionMiddleware` resolves `IRepository`, `IPlayerService`, and `IEncryptionService` to locate the player and decrypt the payload. The decrypted values are exposed to downstream model binding and a `HttpContext.Items["RequestEncrypted"]` flag indicates whether decryption succeeded.

### Registration

The middleware is registered in `Program.cs` before MVC runs so all controllers automatically receive decrypted form values. No additional configuration is required beyond the existing service registrations.

### Plain requests

Requests without the encryption envelope are processed unchanged. Controllers can inspect `HttpContext.Items["RequestEncrypted"]` to determine whether a request was decrypted.
