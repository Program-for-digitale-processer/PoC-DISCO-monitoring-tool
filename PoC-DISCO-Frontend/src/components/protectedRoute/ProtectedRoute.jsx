import { useAuth } from "../../utils/auth/AuthProvider.jsx";
import { Navigate } from "react-router-dom";

export default function ProtectedRoute({ children }) {
    const {user} = useAuth();

    if (user === undefined) {
        return <div>Loading...</div>;
    }

    if (user === null) {

        return <Navigate to="/login" replace />;
    }

    return children;
}
