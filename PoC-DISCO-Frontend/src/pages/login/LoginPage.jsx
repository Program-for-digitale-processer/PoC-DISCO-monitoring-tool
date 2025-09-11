// src/pages/auth/LoginPage.jsx
import { useState } from "react";
import LoginForm from "../../components/LoginForm.jsx";
import {useAuth} from "../../utils/auth/AuthProvider.jsx";

export default function LoginPage() {
    const { login } = useAuth();
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);

    const handleLogin = async (credentials) => {
        setLoading(true);
        setError(null);

        try {
            await login(credentials);
            window.location.href = "/";
        } catch (err) {
            setError(err.response?.data?.message || "Login failed");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center">
            <div className="w-full max-w-md">
                <LoginForm onSubmit={handleLogin} />

                {loading && (
                    <p className="text-gray-500 text-sm text-center mt-2">
                        Logging in…
                    </p>
                )}

                {error && (
                    <p className="text-red-500 text-sm text-center mt-4">
                        {error}
                    </p>
                )}
            </div>
        </div>
    );
}
