import {createContext, useContext, useEffect, useLayoutEffect, useState} from "react";
import axios from "axios";

const AuthContext = createContext(undefined);

export const useAuth = () => {
    const authContext = useContext(AuthContext);

    if (!authContext) {
        throw new Error('useAuth must be used within the AuthProvider');
    }
    return authContext;
}

const AuthProvider = ({children}) => {
    const [token, setToken] = useState(null);

    useEffect(() => {
        const fetchMe = async () => {
            try {
                const response = await axios.get('http://localhost:5000/me');
                setToken(response.data.accessToken);
            } catch {
                setToken(null);
            }
        };

        fetchMe();
    }, []);


    useLayoutEffect(() => {
        const authInterceptor = axios.interceptors.request.use((config) => {
            config.headers.Authorization =
                !config._retry && token ? `Bearer ${token}` : config.headers.Authorization;
            return config;
        });


        return () => {
            axios.interceptors.request.eject(authInterceptor);
        };
    }, [token]);

    useLayoutEffect(() => {
        const refreshInterceptor = axios.interceptors.response.use((response) =>
                response,
            async (err) => {
                    const originalRequest = err.config;


            if (err.response.status === 403 &&
            err.response.data.message === 'Unauthorized') {
                try {
                    const response = axios.get('http://localhost:5000/api/refreshToken');

                    setToken(response.data.accessToken);
                } catch
            }




            })
    })

}


