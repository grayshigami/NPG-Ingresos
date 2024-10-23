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
            <h1>Crear usuario</h1>
            <input type="text" placeholder="Nombre" v-model="nuevoElemento.nombre">
            <input type="text" placeholder="Apellido" v-model="nuevoElemento.apellido">
            <input type="text" placeholder="Nombre de usuario" v-model="nuevoElemento.nombreUsuario">
            <input type="text" placeholder="Correo" v-model="nuevoElemento.correo">
            <input type="password" placeholder="Contraseña" v-model="nuevoElemento.contrasena">
            <select id="selectTipoUsuario" v-model="nuevoElemento.tipoUsuario">
                <option :value="0">Usuario</option>
                <option :value="1">Administrador</option>
            </select>
            <button @click="agregarElemento">
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

export default {
    name: 'crear-usuario',
    data() {
        return {
            name: '',
            nuevoElemento: {
                nombre: '',
                apellido: '',
                nombreUsuario: '',
                correo: '',
                contrasena: '',
                tipoUsuario: 0,
                registroId: null,
                actualizacionId: null
            },
            errorMessage: ''
        }
    },
    created() {
        this.nuevoElemento.registroId = localStorage.getItem('user_id');
        this.nuevoElemento.actualizacionId = localStorage.getItem('user_id');
        this.name = localStorage.getItem('name');
    },
    methods: {
        async agregarElemento() {
            try {
                if (!this.nuevoElemento.nombre) {
                    this.errorMessage = "Debes ingresar el nombre";
                    return;
                }

                const regexNumeros = /\d/;

                if (regexNumeros.test(this.nuevoElemento.nombre)) {
                    this.errorMessage = "El nombre no debe tener números";
                    return;
                }

                if (!this.nuevoElemento.apellido) {
                    this.errorMessage = "Debes ingresar el apellido";
                    return;
                }

                if (regexNumeros.test(this.nuevoElemento.apellido)) {
                    this.errorMessage = "El apellido no debe tener números";
                    return;
                }

                if (!this.nuevoElemento.nombreUsuario) {
                    this.errorMessage = "Debes ingresar el nombre de usuario";
                    return;
                }

                if (!this.nuevoElemento.correo) {
                    this.errorMessage = "Debes ingresar el correo";
                    return;
                }

                if (!this.nuevoElemento.contrasena) {
                    this.errorMessage = "Debes ingresar la contraseña";
                    return;
                }

                const response = await axios.post(`https://${ip_address}:7207/api/Usuario`, this.nuevoElemento, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                });

                const nuevoElementoCreado = response.data;
                console.log(response.data);
                console.log(this.nuevoElemento);
                console.log(nuevoElementoCreado);
                this.$emit('nuevo-elemento', nuevoElementoCreado);
                this.nuevoElemento = {
                    nombre: '',
                    apellido: '',
                    nombreUsuario: '',
                    correo: '',
                    contrasena: '',
                    tipoUsuario: 0
                };
                this.$router.back();
            } catch (error) {
                this.errorMessage = error.response.data.message;
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

input, select, button {
    padding: 10px;
    border-radius: 5px;
    margin-bottom: 10px;
    font-size: 16px;

    @media (max-width: 600px) {
        font-size: 12px;
    }
}

select {
    width: 99%;
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