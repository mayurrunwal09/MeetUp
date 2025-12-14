import React, { useEffect, useState } from "react";
import axios from "axios";
import {
  Box,
  Card,
  CardMedia,
  CardContent,
  Typography,
  Button,
  Grid,
  CardActions,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  List,
  ListItem,
  ListItemText,
  CircularProgress,
  Alert,
} from "@mui/material";
import FavoriteIcon from "@mui/icons-material/Favorite";
import FavoriteBorderIcon from "@mui/icons-material/FavoriteBorder";
import ChatBubbleOutlineIcon from "@mui/icons-material/ChatBubbleOutline";
import DeleteOutlineIcon from "@mui/icons-material/DeleteOutline";
import IconButton from "@mui/material/IconButton";
import DeleteIcon from "@mui/icons-material/Delete";
import BASE_URL from "../config/baseUrl";

const Account = () => {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [openDialog, setOpenDialog] = useState(false);
  const [commentText, setCommentText] = useState("");
  const [selectedPostId, setSelectedPostId] = useState(null);

  const [openCommentDialog, setOpenCommentDialog] = useState(false);
  const [selectedComments, setSelectedComments] = useState([]);
  const [selectedPostTitle, setSelectedPostTitle] = useState("");

  const fetchMyPosts = async () => {
    try {
      const token = localStorage.getItem("token");

      const response = await axios.get(
        `${BASE_URL}api/Post/MyPosts`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      setPosts(response.data);
    } catch (error) {
      console.error("Failed to fetch posts:", error);
      setError("Failed to load posts");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchMyPosts();
  }, []);

  const handleLike = async (postId, index) => {
    try {
      const token = localStorage.getItem("token");
      if (!token) {
        setError("User not authenticated");
        return;
      }

      const isCurrentlyLiked = posts[index].isLoginUserLiked;

      await axios.post(
        `${BASE_URL}api/Liked/LikePost`,
        {
          postId: postId,
          isLiked: !isCurrentlyLiked,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );

      fetchMyPosts();
    } catch (err) {
      console.error("Failed to toggle like:", err);
      setError("Error toggling like");
    }
  };

  const handleOpenCommentDialog = (postId) => {
    setSelectedPostId(postId);
    setCommentText("");
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setSelectedPostId(null);
  };

  const handleSubmitComment = async () => {
    try {
      const token = localStorage.getItem("token");
      if (!token || !selectedPostId) return;

      await axios.post(
        `${BASE_URL}api/Comments/AddComments`,
        {
          text: commentText,
          postId: selectedPostId,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );

      handleCloseDialog();
      fetchMyPosts();
    } catch (err) {
      console.error("Failed to submit comment:", err);
      const errorMessage =
        err.response?.data?.message ||
        err.response?.data ||
        "Failed to submit comment";

      alert(errorMessage);
      //setError("Failed to submit comment");
    }
  };

  const handleCommentClick = (comments, title) => {
    setSelectedComments(comments);
    setSelectedPostTitle(title);
    setOpenCommentDialog(true);
  };

  const handleCloseComments = () => {
    setOpenCommentDialog(false);
  };

  const handleDeletePost = async (postId) => {
    if (!window.confirm("Are you sure you want to delete this post?")) return;

    try {
      const token = localStorage.getItem("token");
      await axios.delete(`${BASE_URL}api/Post/${postId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      fetchMyPosts();
    } catch (err) {
      console.error("Failed to delete post:", err);
      setError("Failed to delete post");
    }
  };

    const handleDeleteComment = async (commentId) => {
    try {
      const token = localStorage.getItem("token");
      if (!token) return;

      await axios.delete(`${BASE_URL}api/Comments/${commentId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      fetchMyPosts(); 
    } catch (err) {
      console.error("Failed to delete comment:", err);
      alert("Unable to delete comment.");
    }
  };

  if (loading) return <CircularProgress sx={{ m: 4 }} />;
  if (error) return <Alert severity="error">{error}</Alert>;

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>
        My Posts
      </Typography>

      <Grid container spacing={3}>
        {posts.map((post, index) => (
          <Grid item xs={12} sm={6} md={4} key={post.postId}>
            <Card>
              {post.mediaUrl && (
                <CardMedia
                  component="img"
                  height="200"
                  image={`${BASE_URL}${post.mediaUrl}`}
                  alt={post.title}
                />
              )}
              <CardContent>
                <Typography variant="h6">{post.title}</Typography>
                <Typography
                  variant="body2"
                  color="text.secondary"
                  sx={{ mb: 1 }}
                >
                  {post.content}
                </Typography>

                <Typography variant="body2">Likes: {post.likeCount}</Typography>
                <Typography
                  variant="body2"
                  sx={{ cursor: "pointer", color: "#1976d2" }}
                  onClick={() => handleCommentClick(post.comments, post.title)}
                >
                  Comments: {post.commentCount}
                </Typography>
              </CardContent>
              <CardActions>
                <Button
                  size="small"
                  variant="text"
                  color="error"
                  onClick={() => handleLike(post.postId, index)}
                  sx={{
                    minWidth: 0,
                    padding: "6px",
                  }}
                >
                  {post.isLoginUserLiked ? (
                    <FavoriteIcon sx={{ color: "#d32f2f" }} />
                  ) : (
                    <FavoriteBorderIcon />
                  )}
                </Button>
                <Button
                  size="small"
                  variant="text"
                  color="primary"
                  onClick={() => handleOpenCommentDialog(post.postId)}
                  sx={{
                    minWidth: 0,
                    padding: "6px",
                  }}
                >
                  <ChatBubbleOutlineIcon />
                </Button>

                <Button
                  size="small"
                  variant="text"
                  color="error"
                  onClick={() => handleDeletePost(post.postId)}
                  sx={{
                    minWidth: 0,
                    padding: "6px",
                  }}
                >
                  <DeleteOutlineIcon />
                </Button>
              </CardActions>
            </Card>
          </Grid>
        ))}
      </Grid>

      {/* ✏️ Add Comment Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog}>
        <DialogTitle>Add a Comment</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            multiline
            rows={4}
            variant="outlined"
            placeholder="Write your comment here..."
            value={commentText}
            onChange={(e) => setCommentText(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog} color="inherit">
            Cancel
          </Button>
          <Button
            onClick={handleSubmitComment}
            color="primary"
            variant="contained"
          >
            Submit
          </Button>
        </DialogActions>
      </Dialog>

      {/* 💬 View Comments Dialog */}
      <Dialog
        open={openCommentDialog}
        onClose={handleCloseComments}
        fullWidth
        maxWidth="sm"
      >
        <DialogTitle>Comments for "{selectedPostTitle}"</DialogTitle>
        <DialogContent dividers>
          {selectedComments.length > 0 ? (
            <List>
              {selectedComments.map((comment, idx) => {
                const isLoginUserPost = posts.find(p => p.postId === comment.postId)?.isLoginUserPost ?? false;
                const canDelete =  isLoginUserPost === true;
                return (
                  <ListItem
                    key={idx}
                    secondaryAction={
                      canDelete && (
                        <IconButton
                          edge="end"
                          aria-label="delete"
                          onClick={() => handleDeleteComment(comment.commentId)}
                        >
                          <DeleteIcon />
                        </IconButton>
                      )
                    }
                  >
                    <ListItemText primary={comment.text} />
                  </ListItem>
                );
              })}
            </List>
          ) : (
            <Typography>No comments yet.</Typography>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseComments}>Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Account;
