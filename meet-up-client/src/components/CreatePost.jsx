import React, { useState } from 'react';
import {
  TextField,
  Button,
  Typography,
  Box,
  Card,
  CardContent,
  CircularProgress,
} from '@mui/material';
import axiosInstance from '../utils/axiosInstance';
import { useNavigate } from 'react-router-dom';

const CreatePost = () => {
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [mediaFile, setMediaFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [successMsg, setSuccessMsg] = useState('');
  const [errorMsg, setErrorMsg] = useState('');

  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setSuccessMsg('');
    setErrorMsg('');

    try {
      const formData = new FormData();
      formData.append('title', title);
      formData.append('content', content);
      if (mediaFile) {
        formData.append('mediaFile', mediaFile);
      }

      const response = await axiosInstance.post('Post/create', formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });

      setSuccessMsg(response.data);
      setTitle('');
      setContent('');
      setMediaFile(null);
      navigate('/home-page'); // Optional redirect after success
    } catch (error) {
      setErrorMsg('Failed to create post.');
      console.error('Create post error:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box display="flex" justifyContent="center" mt={5}>
      <Card sx={{ width: 500 }}>
        <CardContent>
          <Typography variant="h5" gutterBottom>
            Create a New Post
          </Typography>

          <form onSubmit={handleSubmit}>
            <TextField
              label="Title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              fullWidth
              margin="normal"
              required
            />

            <TextField
              label="Content"
              value={content}
              onChange={(e) => setContent(e.target.value)}
              fullWidth
              margin="normal"
              multiline
              rows={4}
              required
            />

            <Button variant="outlined" component="label" sx={{ mt: 2 }}>
              Upload Media
              <input
                type="file"
                hidden
                onChange={(e) => setMediaFile(e.target.files[0])}
                required
              />
            </Button>
            {mediaFile && (
              <Typography variant="body2" mt={1}>
                Selected: {mediaFile.name}
              </Typography>
            )}

            <Box mt={3}>
              <Button
                type="submit"
                variant="contained"
                color="primary"
                disabled={loading}
                fullWidth
              >
                {loading ? <CircularProgress size={24} /> : 'Create Post'}
              </Button>
            </Box>

            {successMsg && (
              <Typography color="success.main" mt={2}>
                {successMsg}
              </Typography>
            )}
            {errorMsg && (
              <Typography color="error.main" mt={2}>
                {errorMsg}
              </Typography>
            )}
          </form>
        </CardContent>
      </Card>
    </Box>
  );
};

export default CreatePost;
