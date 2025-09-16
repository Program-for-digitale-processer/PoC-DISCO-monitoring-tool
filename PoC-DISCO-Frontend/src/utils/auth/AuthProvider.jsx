// src/context/AuthContext.jsx
import { createContext, useContext, useEffect, useLayoutEffect, useState } from "react";
import axios from "axios";

const API_URL = "http://localhost:5142";

const AuthContext = createContext(undefined);

// eslint-disable-next-line react-refresh/only-export-components
export const useAuth = () => {
    const authContext = useContext(AuthContext);
    if (!authContext) {
        throw new Error("useAuth must be used within an AuthProvider");
    }
    return authContext;
};

export const AuthProvider = ({ children }) => {
    const [token, setToken] = useState();
    const [user, setUser] = useState();

    useEffect(() => {
        const fetchMe = async () => {
            try {
                const response = await axios.get(`${API_URL}/token`, { withCredentials: true });
                setToken(response.data.accessToken);
                setUser(response.data.user || null);
            } catch {
                setToken(null);
                setUser(null);
            }
        };
        fetchMe();
    }, []);

    useLayoutEffect(() => {
        const authInterceptor = axios.interceptors.request.use((config) => {
            if (token && !config._retry) {
                config.headers.Authorization = `Bearer ${token}`;
            }
            return config;
        });
        return () => {
            axios.interceptors.request.eject(authInterceptor);
        };
    }, [token]);

    useLayoutEffect(() => {
        const refreshInterceptor = axios.interceptors.response.use(
            (res) => res,
            async (err) => {
                const originalRequest = err.config;

                if (
                    err.response?.status === 403 &&
                    err.response?.data?.message === "Unauthorized" &&
                    !originalRequest._retry
                ) {
                    try {
                        const response = await axios.get(`${API_URL}/api/refreshToken`, {
                            withCredentials: true,
                        });

                        const newToken = response.data.accessToken;
                        setToken(newToken);

                        originalRequest.headers.Authorization = `Bearer ${newToken}`;
                        originalRequest._retry = true;

                        return axios(originalRequest);
                    } catch  {
                        setToken(null);
                        setUser(null);
                    }
                }
                return Promise.reject(err);
            }
        );

        return () => {
            axios.interceptors.response.eject(refreshInterceptor);
        };
    }, []);

    const login = async (credentials) => {
        const response = await axios.post(`${API_URL}/api/login`, credentials, {
            withCredentials: true,
        });
        setToken(response.data.accessToken);
        setUser(response.data.user);
    };

    const logout = async () => {
        await axios.post(`${API_URL}/logout`, {}, { withCredentials: true });
        setToken(null);
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ token, user, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};
