<template>
    <div class="general">
        <div class="toolbar">
            <img src="../assets/logo-negro-Sinfondo.png" alt="" width="10%" height="10%">
            <h2 class="b-user">Bienvenido {{ name }}</h2>
            <div class="spacer"></div>
            <button @click="logout">
                <i class="fa-solid fa-power-off"></i>
                Cerrar sesión
            </button>
        </div>
        <div class="form-data">
            <h1>Cambiar contraseña</h1>
            <input type="password" placeholder="Contraseña actual" v-model="actual" required>
            <input type="password" placeholder="Contraseña nueva" v-model="nueva" required>
            <button @click="updateContrasena">
                <i class="fa-solid fa-floppy-disk"></i>
                Guardar
            </button>
            <button @click="goBack">
                <i class="fa-solid fa-arrow-left"></i>
                Volver
            </button>
            <p v-if="message">{{ message }}</p>
        </div>
    </div>
</template>
<script>
import axios from 'axios';
import { ip_address } from '@/constants/ip-laptop';

export default {
    name: 'cambiar-contrasena',
    data() {
        return {
            name: '',
            actual: '',
            nueva: '',
            message: '',
            actualizacionId: null,
            usuarioId: null
        }
    },
    created() {
        this.name = localStorage.getItem('name');
        this.usuarioId = localStorage.getItem('user_id');
    },
    methods: {
        async updateContrasena() {
            try {
                const response = await axios.put(`https://${ip_address}:7207/api/Auth/cambiar-contrasena/${this.usuarioId}`, {
                    ContrasenaActual: this.actual,
                    ContrasenaNueva: this.nueva,
                }, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                });
                this.message = response.data.message;
                this.actual = '';
                this.nueva = '';
                this.$router.back();
            } catch (error) {
                this.message = error.response.data.message || "Error al actualizar contraseña";
            }
        },
        logout() {
            localStorage.removeItem('token');
            window.location.href = '/login-component';
        },
        goBack() {
            this.$router.back();
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

p {
    color: red;
}

.toolbar {
    width: 100%;
    background-color: rgb(21, 96, 130);
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    padding: 10px;
    color: white;
    box-sizing: border-box;

    @media (max-width: 600px) {
        font-size: 10px;
        padding: 3px;
    }
}

.toolbar button {
    border: none;
}

.spacer {
    width: 50%;

    @media (max-width: 600px) {
        width: 0;
    }
}

.b-user {
    margin-left: 20px;
    margin-right: 180px;
    font-weight: bold;
}

h1 {
    margin-bottom: 10px;
}

.form-data {
    margin: 20px auto;
    width: 40%;
    display: flex;
    flex-direction: column;
    text-align: center;

    @media (max-width: 800px) {
        width: 80%;
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
}

button:hover {
    cursor: pointer;
}
</style>