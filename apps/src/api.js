const apiBaseUrl = (import.meta.env.VITE_API_BASE_URL ?? "").replace(/\/$/, "");

export async function apiRequest(path, options) {
  let response;
  try {
    response = await fetch(`${apiBaseUrl}${path}`, {
      ...options,
      headers: {
        Accept: "application/json",
        ...(options?.body ? { "Content-Type": "application/json" } : {}),
        ...options?.headers,
      },
    });
  } catch {
    throw new Error("The navigation service could not be reached. Check that the API is running and try again.");
  }

  if (!response.ok) {
    const problem = await response.json().catch(() => null);
    const message =
      problem?.detail ??
      problem?.message ??
      problem?.title ??
      (response.status === 404
        ? "This building, destination, or QR location could not be found."
        : `The navigation service returned an error (${response.status}).`);
    throw new Error(message);
  }

  return response.json();
}

export const loadBuilding = (tenantSlug) =>
  apiRequest(`/api/public/buildings/${encodeURIComponent(tenantSlug)}`);

export const resolveQrToken = (token) =>
  apiRequest(`/api/public/qr/${encodeURIComponent(token)}`);

export const findRoute = (tenantSlug, routeRequest) =>
  apiRequest(`/api/public/buildings/${encodeURIComponent(tenantSlug)}/routes`, {
    method: "POST",
    body: JSON.stringify(routeRequest),
  });
