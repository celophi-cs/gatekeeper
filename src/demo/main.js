
// Tab switching
const tabRegister = document.getElementById('tab-register');
const tabLogin = document.getElementById('tab-login');
const tabOAuth = document.getElementById('tab-oauth');
const registerForm = document.getElementById('register-form');
const loginForm = document.getElementById('login-form');
const oauthForm = document.getElementById('oauth-form');

function showTab(tab) {
	[registerForm, loginForm, oauthForm].forEach(f => f.style.display = 'none');
	[tabRegister, tabLogin, tabOAuth].forEach(b => b.classList.remove('active'));
	if (tab === 'register') {
		registerForm.style.display = '';
		tabRegister.classList.add('active');
	} else if (tab === 'login') {
		loginForm.style.display = '';
		tabLogin.classList.add('active');
	} else if (tab === 'oauth') {
		oauthForm.style.display = '';
		tabOAuth.classList.add('active');
	}
}
tabRegister.onclick = () => showTab('register');
tabLogin.onclick = () => showTab('login');
tabOAuth.onclick = () => showTab('oauth');

// Registration
document.getElementById('form-register').onsubmit = async (e) => {
	e.preventDefault();
	const form = e.target;
	const data = {
		email: form.email.value,
		name: form.name.value,
		password: form.password.value
	};
	const res = await fetch('/auth/register', {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify(data)
	});
	const result = await res.json();
	document.getElementById('register-result').textContent = result.message || result.error || JSON.stringify(result);
};

// Login
document.getElementById('form-login').onsubmit = async (e) => {
	e.preventDefault();
	const form = e.target;
	const data = {
		email: form.email.value,
		password: form.password.value
	};
	const res = await fetch('/auth/login', {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify(data)
	});
	const result = await res.json();
	document.getElementById('login-result').textContent = result.message || result.error || JSON.stringify(result);
};

// OAuth 2.0 Flow Test
document.getElementById('form-oauth').onsubmit = async (e) => {
	e.preventDefault();
	const form = e.target;
	const clientId = form.client_id.value;
	const redirectUri = form.redirect_uri.value;
	const scope = form.scope.value;
	// Generate PKCE code challenge/verifier
	const codeVerifier = generateRandomString(64);
	const codeChallenge = await pkceChallengeFromVerifier(codeVerifier);
	// Store verifier in sessionStorage for later
	sessionStorage.setItem('pkce_verifier', codeVerifier);
	// Build authorization URL
	const url = `/oauth/authorize?response_type=code&client_id=${encodeURIComponent(clientId)}&redirect_uri=${encodeURIComponent(redirectUri)}&scope=${encodeURIComponent(scope)}&code_challenge=${encodeURIComponent(codeChallenge)}&code_challenge_method=S256`;
	window.location = url;
};

// PKCE helpers
function generateRandomString(length) {
	const charset = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~';
	let result = '';
	const array = new Uint8Array(length);
	window.crypto.getRandomValues(array);
	for (let i = 0; i < array.length; i++) {
		result += charset[array[i] % charset.length];
	}
	return result;
}
async function pkceChallengeFromVerifier(verifier) {
	const encoder = new TextEncoder();
	const data = encoder.encode(verifier);
	const digest = await window.crypto.subtle.digest('SHA-256', data);
	return btoa(String.fromCharCode(...new Uint8Array(digest)))
		.replace(/\+/g, '-')
		.replace(/\//g, '_')
		.replace(/=+$/, '');
}
