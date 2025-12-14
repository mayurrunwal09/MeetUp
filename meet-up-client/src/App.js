// src/App.jsx
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import LoginPage from "./pages/LoginPage";
import ActivePostDetails from "./components/ActivePostDetails";
import ProtectedRoute from "./components/ProtectedRoute";
import CreatePost from "./components/CreatePost";
import Sidebar from "./components/Sidebar";
import { Box } from "@mui/material";
import Account from "./components/Account";
import RegisterPage from "./components/RegisterForm";
function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route
          path="/home-page"
          element={
            <ProtectedRoute>
              <Box sx={{ display: "flex" }}>
                <Sidebar />
                <Box sx={{ flexGrow: 1, p: 3 }}>
                  <ActivePostDetails />
                </Box>
              </Box>
            </ProtectedRoute>
          }
        />
        <Route
          path="/create-post"
          element={
            <ProtectedRoute>
              <Box sx={{ display: "flex" }}>
                <Sidebar />
                <Box sx={{ flexGrow: 1, p: 3 }}>
                  <CreatePost />
                </Box>
              </Box>
            </ProtectedRoute>
          }
        />
        <Route
          path="/account"
          element={
            <ProtectedRoute>
              <Box sx={{ display: "flex" }}>
                <Sidebar />
                <Box sx={{ flexGrow: 1, p: 3 }}>
                  <Account />
                </Box>
              </Box>
            </ProtectedRoute>
          }
        />
      </Routes>
    </Router>
  );
}

export default App;
