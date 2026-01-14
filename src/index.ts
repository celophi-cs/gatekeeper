import path from 'path';
import { fileURLToPath } from 'url';
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
// Entry point for Gatekeeper OAuth 2.0 / OIDC server
import express from 'express';
import mongoose from 'mongoose';
import dotenv from 'dotenv';
import authRoutes from './routes/auth.ts';
import oauthRoutes from './routes/oauth.ts';

// Load environment variables
dotenv.config();


const app = express();
app.use(express.json());

// Serve static demo site before route handlers
app.use('/demo', express.static(path.resolve(__dirname, 'demo')));

// Use authentication routes
app.use('/auth', authRoutes);
// Use OAuth 2.0 routes
app.use('/oauth', oauthRoutes);


const PORT = process.env.PORT || 3000;
const MONGO_URI = process.env.MONGO_URI || 'mongodb://localhost:27017/gatekeeper';
console.log('MONGO_URI:', MONGO_URI);

mongoose.connect(MONGO_URI)
  .then(() => {
    console.log('Connected to MongoDB');
    app.listen(PORT, () => {
      console.log(`Server running on port ${PORT}`);
    });
  })
  .catch((err: any) => {
    console.error('MongoDB connection error:', err);
  });
