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
        <h1>Editar ingreso</h1>
        <input type="text" placeholder="Nombre" name="nombre" v-model="incidenciaLocal.nombre">
        <input type="text" placeholder="Empresa" name="empresa" v-model="incidenciaLocal.empresa">
        <input type="datetime-local" placeholder="Entrada" name="horaEntrada" v-model="incidenciaLocal.horaEntrada">
        <input type="datetime-local" placeholder="Salida" name="horaSalida" v-model="incidenciaLocal.horaSalida">
        <textarea id="com" placeholder="Comentario" v-model="incidenciaLocal.comentario" rows="5"></textarea>
        <button @click="updateIncidencia">
            <i class="fa-solid fa-floppy-disk"></i>
            Guardar
        </button>
        <button @click="goBack()">
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
    name: 'editar-incidencia',
    data() {
        return {
            name: '',
            actualizacionId: null,
            incidenciaLocal: {},
            errorMessage: ''
        }
    },
    created() {
        this.name = localStorage.getItem('name');
        this.actualizacionId = localStorage.getItem('user_id');
    },
    async mounted() {
        const response = await axios.get(`https://${ip_address}:7207/api/Incidencia/${this.$route.query.incidencia}`, {
            headers: {
                Authorization: `Bearer ${localStorage.getItem('token')}`
            }
        });
        this.incidenciaLocal = response.data[0];
        console.log(this.incidenciaLocal);
    },
    methods: {
        async updateIncidencia() {
            try {
                if (!this.incidenciaLocal.nombre) {
                    this.errorMessage = "El nombre no debe quedar vacío";
                    return;
                }

                const regexNumeros = /\d/;

                if (regexNumeros.test(this.incidenciaLocal.nombre)) {
                    this.errorMessage = "El nombre no debe tener números";
                    return;
                }

                this.incidenciaLocal.actualizacionId = this.actualizacionId;
                this.incidenciaLocal.id = this.$route.query.incidencia;
                let request = {...this.incidenciaLocal};
                delete request.id;
                const response = await axios.put(`https://${ip_address}:7207/api/Incidencia/${this.incidenciaLocal.id}`, request, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                });

                console.log("Incidencia actualizada:", response.data);
                this.$router.back();
            } catch (error) {
                console.error(error);
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

 #com {
    font-family: sans-serif;
    font-size: 16px;
    margin-bottom: 10px;
    width: 100%;
    padding: 10px;
    border-radius: 5px;
    border: 2px solid black;
    box-sizing: border-box;
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