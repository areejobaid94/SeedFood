// Import the functions you need from the SDKs you need
// Import the functions you need from the SDKs you need
importScripts('https://www.gstatic.com/firebasejs/9.0.1/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/9.0.1/firebase-messaging-compat.js');

// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
firebase.initializeApp({
  apiKey: "AIzaSyB96mmrshd56OaGPZNPvi6l1NN2yUa8ua8",
  authDomain: "info-seed-prod.firebaseapp.com",
  projectId: "info-seed-prod",
  storageBucket: "info-seed-prod.appspot.com",
  messagingSenderId: "174110793202",
  appId: "1:174110793202:web:87412ae4ece27e21d09f6b",
  measurementId: "G-C1ETYG243Y"
});

// Initialize Firebase
const messaging = firebase.messaging();



