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
            <h1>Editar usuario</h1>
            <input type="text" placeholder="Nombre" v-model="usuarioLocal.nombre">
            <input type="text" placeholder="Apellido" v-model="usuarioLocal.apellido">
            <input type="text" placeholder="Nombre de usuario" v-model="usuarioLocal.nombreUsuario">
            <input type="text" placeholder="Correo" v-model="usuarioLocal.correo">
            <select id="selectTipoUsuario" v-model="usuarioLocal.tipoUsuario">
                <option :value="0">Usuario</option>
                <option :value="1">Administrador</option>
            </select>
            <input type="password" placeholder="Contraseña" v-model="usuarioLocal.contrasena">
            <button @click="updateUsuario">
                <i class="fa-solid fa-floppy-disk"></i>
                Guardar
            </button>
            <button @click="goBack">
                <i class="fa-solid fa-arrow-left"></i>
                Volver
            </button>
            <p v-if="errorMessage">{{ errorMessage }}</p>
        </div>
    </div>
</template>
<script>
import { ip_address } from '@/constants/ip-laptop';
import axios from 'axios';
import bcrypt from 'bcryptjs';

export default {
    name: 'editar-usuario',
    data() {
        return {
            name: '',
            actualizacionId: null,
            usuarioLocal: {},
            errorMessage: '',
            contActual: ''
        }
    },
    created() {
        this.name = localStorage.getItem('name');
        this.actualizacionId = localStorage.getItem('user_id');
    },
    async mounted() {
        const response = await axios.get(`https://${ip_address}:7207/api/Usuario/${this.$route.query.usuario}`, {
            headers: {
                Authorization: `Bearer ${localStorage.getItem('token')}`
            }
        });
        this.usuarioLocal = response.data[0];
        console.log(this.usuarioLocal);
    },
    methods: {
        async updateUsuario() {
            try {
                if (!this.usuarioLocal.nombre) {
                    this.errorMessage = "El nombre no debe quedar vacío";
                    return;
                }

                const regexNumeros = /\d/;

                if (regexNumeros.test(this.usuarioLocal.nombre)) {
                    this.errorMessage = "El nombre no debe tener números";
                    return;
                }

                if (!this.usuarioLocal.apellido) {
                    this.errorMessage = "El apellido no debe quedar vacío";
                    return;
                }

                if (regexNumeros.test(this.usuarioLocal.apellido)) {
                    this.errorMessage = "El apellido no debe tener números";
                    return;
                }

                if (!this.usuarioLocal.nombreUsuario) {
                    this.errorMessage = "El nombre de usuario no debe quedar vacío";
                    return;
                }

                if (!this.usuarioLocal.correo) {
                    this.errorMessage = "El correo no debe quedar vacío";
                    return;
                }

                if (!this.usuarioLocal.contrasena) {
                    this.errorMessage = "La contraseña no debe quedar vacía";
                    return;
                }

                this.usuarioLocal.actualizacionId = this.actualizacionId;
                this.usuarioLocal.id = this.$route.query.usuario;

                let request = {...this.usuarioLocal};
                delete request.id;
                await axios.put(`https://${ip_address}:7207/api/Usuario/${this.usuarioLocal.id}`, request, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                });
                this.$router.back();
            } catch (error) {
                this.errorMessage = error.response.data.message;
            }
        },
        goToContrasena() {
            this.$router.push('/cambiar-contrasena');
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

input, button, select {
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