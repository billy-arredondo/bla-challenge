import { useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import { api, ApiError } from "../lib/api";
import { useSession } from "../stores";

export function SessionHydrator() {
  const token = useSession((state) => state.token);
  const setSession = useSession((state) => state.setSession);
  const clear = useSession((state) => state.clear);
  const query = useQuery({
    queryKey: ["me"],
    queryFn: api.me,
    enabled: Boolean(token),
    retry: false,
  });

  useEffect(() => {
    if (query.data && token) {
      setSession(token, query.data);
    }

    if (query.error instanceof ApiError && query.error.status === 401) {
      clear();
    }
  }, [clear, query.data, query.error, setSession, token]);

  return null;
}
