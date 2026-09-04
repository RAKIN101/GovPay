export const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5145";

export type StoredSession = {
  username: string;
  email: string;
  role: string;
  token: string;
};

export function getStoredSession(): StoredSession | null {
  if (typeof window === "undefined") return null;

  const raw = window.localStorage.getItem("govpay_session");
  if (!raw) return null;

  try {
    return JSON.parse(raw) as StoredSession;
  } catch {
    return null;
  }
}

export function setStoredSession(session: StoredSession) {
  if (typeof window === "undefined") return;
  window.localStorage.setItem("govpay_session", JSON.stringify(session));
}

export function clearStoredSession() {
  if (typeof window === "undefined") return;
  window.localStorage.removeItem("govpay_session");
}

export async function request<T>(path: string, options: RequestInit = {}, requireAuth = false): Promise<T> {
  const headers = new Headers(options.headers ?? {});

  if (!(options.body instanceof FormData)) {
    headers.set("Content-Type", "application/json");
  }

  if (requireAuth) {
    const session = getStoredSession();
    if (!session?.token) {
      throw new Error("Authentication required");
    }
    headers.set("Authorization", `Bearer ${session.token}`);
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers,
    cache: "no-store",
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || "Request failed");
  }

  const contentType = response.headers.get("content-type") || "";
  if (contentType.includes("application/json")) {
    return (await response.json()) as T;
  }

  return null as T;
}
