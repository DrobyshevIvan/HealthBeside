import { useCallback, useState } from "react";
import { profileService } from "../services/profileService";

export function useUpdateProfile() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const updateUserProfile = useCallback(async (profileData) => {
    setLoading(true);
    setError(null);

    try {
      const result = await profileService.updateProfileInfo(profileData);
      return result;
    } catch (e) {
      setError(e);
      throw e;
    } finally {
      setLoading(false);
    }
  }, []);

  return { loading, error, updateUserProfile };
}
