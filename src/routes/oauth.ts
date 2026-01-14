


import express from 'express';
import crypto from 'crypto';
import Client from '../models/Client.ts';
import User from '../models/User.ts';
import Token from '../models/Token.ts';
import { signAccessToken, signRefreshToken, verifyToken } from '../utils/jwt.ts';

const router = express.Router();

// In-memory store for auth codes (for demo; production should use DB)
const authCodes: Record<string, any> = {};

// Authorization endpoint
router.get('/authorize', async (req, res) => {
  const { response_type, client_id, redirect_uri, scope, state, code_challenge, code_challenge_method } = req.query;
  if (response_type !== 'code') {
    return res.status(400).json({ error: 'unsupported_response_type' });
  }
  const client = await Client.findOne({ clientId: client_id });
  if (!client || !client.redirectUris.includes(redirect_uri as string)) {
    return res.status(400).json({ error: 'invalid_client' });
  }
  // For demo: show a simple login form (in real app, redirect to login UI)
  res.send(`<form method='POST' action='/oauth/authorize'>
    <input type='hidden' name='client_id' value='${client_id}' />
    <input type='hidden' name='redirect_uri' value='${redirect_uri}' />
    <input type='hidden' name='scope' value='${scope}' />
    <input type='hidden' name='state' value='${state}' />
    <input type='hidden' name='code_challenge' value='${code_challenge}' />
    <input type='hidden' name='code_challenge_method' value='${code_challenge_method}' />
    <input type='email' name='email' placeholder='Email' required />
    <input type='password' name='password' placeholder='Password' required />
    <button type='submit'>Authorize</button>
  </form>`);
});

// Handle login and issue code
router.post('/authorize', async (req, res) => {
  const { email, password, client_id, redirect_uri, scope, state, code_challenge, code_challenge_method } = req.body;
  const user = await User.findOne({ email });
  if (!user) return res.status(401).send('Invalid credentials');
  const bcrypt = require('bcrypt');
  const valid = await bcrypt.compare(password, user.password);
  if (!valid) return res.status(401).send('Invalid credentials');
  // Generate code
  const code = crypto.randomBytes(32).toString('hex');
  authCodes[code] = {
    userId: user._id,
    clientId: client_id,
    redirectUri: redirect_uri,
    scope,
    codeChallenge: code_challenge,
    codeChallengeMethod: code_challenge_method,
    createdAt: Date.now()
  };
  // Redirect back with code
  const url = new URL(redirect_uri);
  url.searchParams.set('code', code);
  if (state) url.searchParams.set('state', state);
  res.redirect(url.toString());
});

// Token endpoint
router.post('/token', async (req, res) => {
  const { grant_type, code, redirect_uri, client_id, code_verifier } = req.body;
  if (grant_type !== 'authorization_code') {
    return res.status(400).json({ error: 'unsupported_grant_type' });
  }
  const authCode = authCodes[code];
  if (!authCode || authCode.redirectUri !== redirect_uri || authCode.clientId !== client_id) {
    return res.status(400).json({ error: 'invalid_grant' });
  }
  // PKCE check
  if (authCode.codeChallengeMethod === 'S256') {
    const hash = crypto.createHash('sha256').update(code_verifier).digest();
    const challenge = hash.toString('base64url');
    if (challenge !== authCode.codeChallenge) {
      return res.status(400).json({ error: 'invalid_grant', error_description: 'PKCE verification failed' });
    }
  }
  // Issue JWT access and refresh tokens
  const accessToken = signAccessToken({
    sub: String(authCode.userId),
    client_id,
    scope: authCode.scope
  }, '1h');
  const refreshToken = signRefreshToken({
    sub: String(authCode.userId),
    client_id,
    scope: authCode.scope
  }, '7d');
  // Save token in DB
  await Token.create({
    userId: authCode.userId,
    clientId: client_id,
    accessToken,
    refreshToken,
    scope: authCode.scope,
    expiresAt: new Date(Date.now() + 3600 * 1000)
  });
  // Remove used code
  delete authCodes[code];
  res.json({
    access_token: accessToken,
    token_type: 'Bearer',
    expires_in: 3600,
    refresh_token: refreshToken
  });
});

// Token introspection endpoint (validate access token)
router.post('/introspect', async (req, res) => {
  const { token } = req.body;
  const payload = verifyToken(token);
  if (!payload) {
    return res.json({ active: false });
  }
  res.json({ active: true, ...payload });
});

export default router;
