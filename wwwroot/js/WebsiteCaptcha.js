(function() {
    const CLIENT_ID = '689524953311-dr0qcsstc7gtp0pt3cth97n1c53p3giv.apps.googleusercontent.com';
  const SCOPE = 'https://www.googleapis.com/auth/gmail.send';

  let tokenClient = null;
  let accessToken = null;

  gapi.load('client',async () => {
          await gapi.client.load('gmail', 'v1');
          console.log('Gmail API loaded');
      });
  
  

  function handleAuth() {
  return new Promise((resolve, reject) => {
    if (!tokenClient) {
      tokenClient = google.accounts.oauth2.initTokenClient({
        client_id: CLIENT_ID,
        scope: SCOPE,
        callback: (tokenResponse) => {
          if (tokenResponse.access_token) {
            accessToken = tokenResponse.access_token;
            resolve();
          } else {
            reject("Failed to get access token");

          }
        }
      });
    }
    tokenClient.requestAccessToken();
  });
}

  function isValidEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(String(email).toLowerCase());
  }

  /**
   * Sanitizes a string input by removing any HTML tags and trimming it.
   * @param {string} input - The string input to sanitize.
   * @returns {string} The sanitized string.
   */


  function sanitizeInput(input) {
    const div = document.createElement('div');
    div.textContent = input; // This will escape any HTML tags
    return div.innerHTML; // Trim whitespace
  }

  let canSubmit = true;
  document.getElementById('emailForm').onsubmit = async function(event) {
    event.preventDefault();

    // apply rate-limiting and reCAPTCHA
    if (!canSubmit) {
      document.getElementById('error-message').textContent = 'Please wait 10seconds before submitting again.';
      document.getElementById('error-message').style.display = 'block';
      return;
    }

    const isLocalhost = location.hostname === 'localhost' || location.hostname === '127.0.0.1';
    //To add CAPTCHA to your contact form -
    const recaptchaResponse = grecaptcha.getResponse();
    if (!isLocalhost &&!recaptchaResponse ) {
      document.getElementById('error-message').textContent = 'Please verify the CAPTCHA.';
      document.getElementById('error-message').style.display = 'block';
      return;
    } else {

      canSubmit = false; // Disable further submissions for 10 seconds
      setTimeout(() => {
        canSubmit = true; // Re-enable submissions after 10 seconds
      }, 10000);
      document.getElementById('error-message').style.display = 'none';
    }

    if (!accessToken) {
      try {
        await handleAuth();
        
      } catch (error) {
          document.getElementById('error-message').textContent = 'Authentication failed. Please try again.';
          document.getElementById('error-message').style.display = 'block';
          return;
      }
      
      
    }
    const form = event.target;
    const fromEmail = sanitizeInput(form.email.value);
    if (!isValidEmail(fromEmail)) {
      document.getElementById('error-message').textContent = 'Please enter a valid email address.';
      document.getElementById('error-message').style.display = 'block';
      return;
    } 
   
    const to = 'oanhthuytran098@gmail.com';
    const subject = sanitizeInput(form.subject.value);
    const message = sanitizeInput(form.message.value);
    
    if (message.length > 1000) {
      document.getElementById('error-message').textContent = 'Message is too long. Please limit it to 1000 characters.';
      document.getElementById('error-message').style.display = 'block';
      return;
    }
    const emailContent = [
      `From: ${fromEmail}`,
      `To: ${to}`,
      `Reply-To: ${fromEmail}`,  
      `Subject: ${subject}`,
      '', 
      'Sender Email: ' + fromEmail,
      message
    ].join('\n');

    const base64EncodedEmail = btoa(emailContent).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    await gapi.client.init({
      apiKey:''});
      gapi.auth.setToken({ access_token: accessToken });

      try {
        const response = await gapi.client.gmail.users.messages.send({
          userId: 'me',
          resource: {
            raw: base64EncodedEmail
          }
        });
        console.log('Email sent successfully:', response);
        document.getElementById('sent-message').style.display = 'block';
        document.getElementById('error-message').style.display = 'none';
        setTimeout(() => {
          document.getElementById('sent-message').style.display = 'none';
        }, 10000);
        form.reset(); // Reset the form after successful submission
      } catch (error) {
        console.error('Error sending email:', error);
        document.getElementById('error-message').textContent = 'Failed to send email. Please try again.';
        document.getElementById('error-message').style.display = 'block';
      }
    
      
  };
})()