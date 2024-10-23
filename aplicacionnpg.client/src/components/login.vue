<template>
    <div class="form-data">
        <img src="../assets/logo-negro-Sinfondo.png" width="250" height="120">
        <h1>Iniciar sesión</h1>
        <input type="text" placeholder="Usuario" v-model="nombreUsuario">
        <input type="password" placeholder="Contraseña" v-model="contrasena">
        <button @click="login">
            <i class="fa-solid fa-user"></i>
            Iniciar sesión
        </button>
        <div class="err" v-if="errorMessage">{{ errorMessage }}</div>
    </div>
</template>
<script>
import { ip_address } from '@/constants/ip-laptop';
import axios from 'axios';

export default {
    name: 'login-component',
    data() {
        return {
            nombreUsuario: '',
            contrasena: '',
            errorMessage: ''
        }
    },
    methods: {
        async login() {
            try {
                const response = await axios.post(`https://${ip_address}:7207/api/Auth/login`, {
                    nombreUsuario: this.nombreUsuario,
                    contrasena: this.contrasena
                });
                console.log("Response_login:", response);
                const { access_token, user_type, name, user_id, register_id } = response.data;
                localStorage.setItem('token', access_token);
                localStorage.setItem('user_type', user_type);
                localStorage.setItem('name', name);
                localStorage.setItem('user_id', user_id);
                localStorage.setItem('register_id', register_id);
                console.log("Token:", access_token);

                if (user_type == 0) {
                    this.$router.push('/pantalla-usuario');
                } else {
                    this.$router.push('/pantalla-administrador');
                }
                console.log('userId:', user_id);
            } catch (error) {
                console.log("Error:", error);
                this.errorMessage = "Usuario o contraseña invalidos";
            }
        }
    }
}
</script>
<style>
* {
    font-family: sans-serif;
    margin: 0;
    padding: 0;
    overflow-x: hidden;

    @media (max-width: 600px) {
        font-size: 12px;
    }
}

.err {
    color: red;
}

h1 {
    margin-bottom: 10px;
}

.form-data {
    margin: 100px auto;
    width: 40%;
    display: flex;
    flex-direction: column;
    text-align: center;

    @media (max-width: 800px) {
        width: 80%;
    }
}

img {
    align-self: center;
    
    @media (max-width: 800px) {
        width: 30%;
        height: 30%;
    }
}

input, button {
    padding: 10px;
    border-radius: 5px;
    margin-bottom: 10px;
    font-size: 16px;

    @media (max-width: 600px) {
        font-size: 12px;
    }
}

button {
    color: white;
    background-color: rgb(21, 96, 130);
    font-weight: bold;

    @media (max-width: 600px) {
        font-size: 12px;
    }
}

button:hover {
    cursor: pointer;
}
</style>