// src/components/Sidebar.jsx
import React from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import {
  Box,
  List,
  ListItem,
  ListItemText,
  Divider,
  Typography,
} from "@mui/material";
import axios from "axios";
import BASE_URL from "../config/baseUrl";

const Sidebar = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const navItems = [
    { path: "/home-page", label: "Home" },
    { path: "/create-post", label: "Create Post" },
    { path: "/account", label: "Account" },
  ];

  const handleLogout = async () => {
    try {
      await axios.post(
        `${BASE_URL}api/Auth/logout`,
        {},
        { withCredentials: true }
      );
      localStorage.removeItem('token');
      navigate("/"); 
    } catch (error) {
      console.error("Logout failed:", error);
    }
  };

  return (
    <Box
      sx={{
        width: 240,
        height: "100vh",
        backgroundColor: "#f5f5f5",
        p: 2,
        boxShadow: 1,
      }}
    >
      <Typography variant="h6" gutterBottom>
        Dashboard
      </Typography>
      <Divider sx={{ mb: 2 }} />
      <List>
        {navItems.map((item) => (
          <ListItem
            button
            key={item.path}
            component={Link}
            to={item.path}
            selected={location.pathname === item.path}
          >
            <ListItemText primary={item.label} />
          </ListItem>
        ))}
        <Divider sx={{ my: 2 }} />
        <ListItem button onClick={handleLogout}>
          <ListItemText primary="Logout" sx={{ color: "red" }} />
        </ListItem>
      </List>
    </Box>
  );
};

export default Sidebar;
