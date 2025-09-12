import {Route, Routes} from "react-router-dom";
import LoginPage from "../pages/login/LoginPage.jsx";
import DashboardPage from "../pages/dashboard/DashboardPage.jsx";
import ProtectedRoute from "../components/protectedRoute/ProtectedRoute.jsx";

export default function AppRoutes () {
    return (
        <Routes>
            <Route path={'/login'} element={<LoginPage/>}/>
            <Route path={'/'} element={<ProtectedRoute><DashboardPage/></ProtectedRoute>}/>
        </Routes>
    )
}