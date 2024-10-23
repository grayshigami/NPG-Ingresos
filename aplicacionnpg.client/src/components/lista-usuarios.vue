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
        <div class="screen-data">
            <button @click="goToCreate">
                <i class="fa-solid fa-plus"></i>
                Crear usuario
            </button>
            <button @click="showAllUsers">
                <i class="fa-solid fa-eye"></i>
                {{ showAll ? 'Ver activos': 'Ver inactivos' }}
            </button>
        </div>
        <CrearUsuario @nuevoElemento="agregarElemento" />
        <div class="table-data">
            <table>
                <thead>
                    <tr>
                    <th>Nombre</th>
                    <th>Apellido</th>
                    <th>Nombre de usuario</th>
                    <th>Correo</th>
                    <th>Tipo de usuario</th>
                    <th>Estado</th>
                    <th>Opciones</th>
                </tr>
                </thead>
                <tbody>
                    <tr v-for="usuario in usuarios" :key="usuario.id">
                        <td>{{ usuario.nombre }}</td>
                        <td>{{ usuario.apellido }}</td>
                        <td>{{ usuario.nombreUsuario }}</td>
                        <td>{{ usuario.correo }}</td>
                        <td>{{ usuario.tipoUsuario == 0 ? 'Usuario' : 'Administrador' }}</td>
                        <td>{{ usuario.estado === 1 ? 'Activo' : 'Inactivo' }}</td>
                        <td>
                            <button @click="editUsuario(usuario)" :disabled="usuario.estado === 0">
                                <i class="fa-solid fa-pencil"></i>
                            </button>
                            <button @click="toggleEstado(usuario)">
                                <i class="fa-solid fa-power-off"></i>
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
            <EditarUsuario v-if="userToEdit" :usuario="userToEdit"
                @usuario-updated="onUsuarioUpdated" />
        </div>
    </div>
</template>
<script>
import axios from 'axios';
import CrearUsuario from './crear-usuario.vue';
import EditarUsuario from './editar-usuario.vue';
import { ip_address } from '@/constants/ip-laptop';

export default {
    name: 'lista-usuarios',
    components: CrearUsuario, EditarUsuario,
    data() {
        return {
            usuarios: [],
            name: '',
            showAll: false
        }
    },
    created() {
        this.name = localStorage.getItem('name');
    },
    mounted() {
        this.getUsuarios();
    },
    methods: {
        async getUsuarios() {
            try {
                let response;
                
                if (this.showAll) {
                    response = await axios.get(`https://${ip_address}:7207/api/Usuario`, {
                        headers: {
                            Authorization: `Bearer ${localStorage.getItem('token')}`
                        }
                    });
                } else {
                    response = await axios.get(`https://${ip_address}:7207/api/Usuario/usuarios-activos`, {
                        params: { estado: 1 },
                        headers: {
                            Authorization: `Bearer ${localStorage.getItem('token')}`
                        }
                    });
                }

                this.usuarios = response.data;
            } catch (error) {
                console.error(error);
            }
        },
        showAllUsers() {
            this.showAll = !this.showAll;
            this.getUsuarios();
        },
        editUsuario(usuario) {
            this.$router.push({name: 'editar-usuario', query: {usuario: usuario.id}});
        },
        async toggleEstado(usuario) {
            console.log(usuario.estado);
            try {
                await axios.put(`https://${ip_address}:7207/api/Usuario/cambiar-estado/${usuario.id}`, null, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                });
                this.getUsuarios();
            } catch (error) {
                console.error(error);
            }
            console.log(usuario.estado);
        },
        async agregarElemento(nuevoElemento) {
            this.usuarios.push(nuevoElemento);
        },
        logout() {
            localStorage.removeItem('token');
            window.location.href = '/login-component';
        },
        goToCreate() {
            this.$router.push('/crear-usuario')
        },
        goToEdit() {
            this.$router.push('/editar-usuario')
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
        overflow-x: scroll;
    }
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
    height: 45px;
    border: none;
}

.spacer {
    width: 50%;

    @media (max-width: 600px) {
        width: 0;
    }
}

.usuario-header {
    display: flex;
    justify-content: center;
    margin: 10px;
}

.b-user {
    margin-left: 20px;
    margin-right: 200px;
    font-weight: bold;
}

.general {
    display: flex;
    flex-direction: column;
    text-align: center;
}

button {
    color: white;
    background-color: rgb(21, 96, 130);
    font-weight: bold;
    padding: 10px;
    border-radius: 5px;
    margin-bottom: 10px;
    font-size: 16px;

    @media (max-width: 600px) {
        font-size: 12px;
    }
}

input, button {
    margin-right: 10px;
}

button:hover {
    cursor: pointer;
}

table {
    margin: 30px auto;
    text-align: left;
    font-size: 16px;
    background-color: rgb(249, 249, 249);
    border-radius: 8px;
}

th, td {
    padding: 8px;

    @media (max-width: 600px) {
        padding: 4px;
    }
}

td {
    border-top: 1px solid black;
}
</style>