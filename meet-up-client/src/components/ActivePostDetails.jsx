import React, { useEffect, useState } from "react";
import axios from "axios";
import {
  Card,
  CardContent,
  CardMedia,
  Typography,
  Button,
  Grid,
  Box,
  CardActions,
  Alert,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  List,
  ListItem,
  ListItemText,
} from "@mui/material";
import IconButton from "@mui/material/IconButton";
import DeleteIcon from "@mui/icons-material/Delete";
import FavoriteIcon from "@mui/icons-material/Favorite";
import FavoriteBorderIcon from "@mui/icons-material/FavoriteBorder";
import ChatBubbleOutlineIcon from "@mui/icons-material/ChatBubbleOutline";
import BASE_URL from "../config/baseUrl";

const ActivePostDetails = () => {
  const [posts, setPosts] = useState([]);
  const [error, setError] = useState("");

  const [openDialog, setOpenDialog] = useState(false);
  const [commentText, setCommentText] = useState("");
  const [selectedPostId, setSelectedPostId] = useState(null);

  const [openCommentDialog, setOpenCommentDialog] = useState(false);
  const [selectedComments, setSelectedComments] = useState([]);
  const [selectedPostTitle, setSelectedPostTitle] = useState("");
  const [replyingTo, setReplyingTo] = useState(null); // commentId of the comment we're replying to
  const [replyText, setReplyText] = useState("");


  const fetchActivePosts = async () => {
    try {
      const token = localStorage.getItem("token");
      if (!token) {
        setError("User not authenticated");
        return;
      }

      const response = await axios.get(
        `${BASE_URL}api/Post/ActivePostDetails`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      setPosts(response.data);
    } catch (err) {
      console.error("Failed to fetch active post details:", err);
      setError("Failed to load posts");
    }
  };

  useEffect(() => {
    fetchActivePosts();
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

      fetchActivePosts(); // Refresh data
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
      fetchActivePosts(); // Refresh comment count
    } catch (err) {
      console.error("Failed to submit comment:", err);
      // Extract error message from response
      const errorMessage =
        err.response?.data?.message ||
        err.response?.data ||
        "Failed to submit comment";

      alert(errorMessage);
      // setError("Failed to submit comment");
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

  const handleDeleteComment = async (commentId) => {
    try {
      const token = localStorage.getItem("token");
      if (!token) return;

      await axios.delete(`${BASE_URL}api/Comments/Delete/${commentId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      fetchActivePosts();
      setSelectedComments((prev) =>
      prev.filter((c) => c.commentId !== commentId)
    );
    } catch (err) {
      console.error("Failed to delete comment:", err);
      alert("Unable to delete comment.");
    }
  };

  const handleSubmitChildComment = async (parentCommentId, postId) => {
    try {
      const token = localStorage.getItem("token");
      if (!token) return;

      await axios.post(
        `${BASE_URL}api/ChildComments/AddChildComments`,
        {
          parentCommentId: parentCommentId,
          text: replyText,
          postId: postId,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );

      setReplyingTo(null);
      setReplyText("");
      fetchActivePosts(); 
    } catch (err) {
      console.error("Failed to add child comment:", err);
      alert("Unable to add reply.");
    }
  };

    const handleDeleteChildComment = async (commentId) => {
    try {
      const token = localStorage.getItem("token");
      if (!token) return;

      await axios.delete(`${BASE_URL}api/ChildComments/Delete/${commentId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      fetchActivePosts();
      setSelectedComments((prev) =>
      prev.filter((c) => c.commentId !== commentId)
    );
    } catch (err) {
      console.error("Failed to delete comment:", err);
      alert("Unable to delete comment.");
    }
  };

  return (
    <Box p={4}>
      <Typography variant="h4" gutterBottom>
        Active Post Details
      </Typography>

      {error && <Alert severity="error">{error}</Alert>}

      <Grid container spacing={3}>
        {posts.length > 0
          ? posts.map((post, index) => (
            <Grid item xs={12} sm={6} md={4} key={index}>
              <Card>
                <CardMedia
                  component="img"
                  height="200"
                  image={`${BASE_URL}${post.mediaUrl}`}
                  alt={post.title}
                />
                <CardContent>
                  <Typography variant="h6">{post.title}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    Content: {post.content}
                  </Typography>
                  <Typography variant="body2">
                    Likes: {post.likeCount}
                  </Typography>
                  <Typography
                    variant="body2"
                    sx={{ cursor: "pointer", color: "#1976d2" }}
                    onClick={() =>
                      handleCommentClick(post.comments, post.title)
                    }
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
                </CardActions>
              </Card>
            </Grid>
          ))
          : !error && <Typography>No active posts found.</Typography>}
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
                const isLoginUserPost =
                  posts.find((p) => p.postId === comment.postId)?.isLoginUserPost ?? false;
                const canDelete =
                  comment.isCommentHost === true || isLoginUserPost === true;

                return (
                  <React.Fragment key={idx}>
                    {/* Parent Comment */}
                    <ListItem
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
                      <ListItemText
                        primary={comment.text}
                        secondary={
                          <Typography
                            variant="body2"
                            sx={{ color: "primary.main", cursor: "pointer" }}
                            onClick={() => {
                              setReplyingTo(comment.commentId);
                              setReplyText("");
                            }}
                          >
                            Reply
                          </Typography>
                        }
                      />

                    </ListItem>
                    {replyingTo === comment.commentId && (
                      <Box sx={{ pl: 4, mt: 1, display: "flex", gap: 1 }}>
                        <TextField
                          size="small"
                          placeholder="Write your reply..."
                          value={replyText}
                          onChange={(e) => setReplyText(e.target.value)}
                          fullWidth
                        />
                        <Button
                          variant="contained"
                          size="small"
                          onClick={() =>
                            handleSubmitChildComment(comment.commentId, comment.postId)
                          }
                        >
                          Send
                        </Button>
                      </Box>
                    )}

                    {/* Child Comments */}
                    {comment.childComments && comment.childComments.length > 0 && (
                      <List sx={{ pl: 4 }}>
                        {comment.childComments
                          .filter(
                            (child) => child.parentCommentId === comment.commentId
                          )
                          .map((child, cIdx) => {
                            const childCanDelete =
                              child.isCommentHost === true || isLoginUserPost === true;

                            return (
                              <ListItem
                                key={cIdx}
                                secondaryAction={
                                  childCanDelete && (
                                    <IconButton
                                      edge="end"
                                      aria-label="delete"
                                      onClick={() =>
                                        handleDeleteChildComment(child.childCommentId)
                                      }
                                    >
                                      <DeleteIcon fontSize="small" />
                                    </IconButton>
                                  )
                                }
                              >
                                <ListItemText
                                  primary={child.text}
                                  secondary={
                                    <Typography
                                      variant="body2"
                                      sx={{ color: "primary.main", cursor: "pointer" }}
                                      onClick={() => {
                                        setReplyingTo(child.childCommentId);
                                        setReplyText("");
                                      }}
                                    >
                                      Reply
                                    </Typography>
                                  }
                                />

                              </ListItem>
                            );
                          })}
                      </List>
                    )}
                  </React.Fragment>
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

export default ActivePostDetails;