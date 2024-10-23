<template>
    <div v-if="isAdmin">
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
                <input type="date" v-model="selectedDate" @change="filterIncidencias">
                <button @click="goToIncidencia">
                    <i class="fa-solid fa-plus"></i>
                    Registrar ingreso
                </button>
                <button @click="exportToPDF">
                    <i class="fa-solid fa-file-export"></i>
                    Exportar
                </button>
                <button @click="goToList">
                    <i class="fa-solid fa-list"></i>
                    Ver lista de usuarios
                </button>
                <button @click="goToContrasena">Cambiar contraseña</button>
            </div>
            <RegistrarIncidencia @nuevoElemento="agregarElemento" :userId="userId" />
            <div class="table-data">
                <table id="table-export">
                    <thead>
                        <tr>
                        <th>Nombre</th>
                        <th>Empresa</th>
                        <th>Entrada</th>
                        <th>Salida</th>
                        <th>Comentario</th>
                        <th>Acciones</th>
                    </tr>
                    </thead>
                    <tbody>
                        <tr v-for="incidencia in filteredIncidencias" :key="incidencia.id">
                            <td>{{ incidencia.nombre }}</td>
                            <td>{{ incidencia.empresa }}</td>
                            <td>{{ formatDate(incidencia.horaEntrada) }}</td>
                            <td>{{ incidencia.horaSalida ? formatDate(incidencia.horaSalida) : null }}</td>
                            <td class="comm">{{ incidencia.comentario }}</td>
                            <td>
                                <button @click="editIncidencia(incidencia)"><i class="fa-solid fa-pencil"></i></button>
                                <button @click="eliminarElemento(incidencia.id)"><i class="fa-solid fa-trash"></i></button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>
<script>
import RegistrarIncidencia from './registrar-incidencia.vue';
import { Date } from 'core-js';
import jsPDF from 'jspdf';
import { ip_address } from '@/constants/ip-laptop';
import axios from 'axios';
import { base64url } from '../constants/base64';

export default {
    name: 'pantalla-administrador',
    components: RegistrarIncidencia,
    data() {
        return {
            incidencias: [],
            name: '',
            selectedDate: new Date().toISOString().substring(0, 10),
            filteredIncidencias: [],
            isAdmin: false
        }
    },
    created() {
        this.name = localStorage.getItem('name');
        this.checkAdmin();
    },
    mounted() {
        this.fetchIncidencias();
    },
    methods: {
        async fetchIncidencias() {
            try {
                const response = await axios.get(`https://${ip_address}:7207/api/Incidencia`, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                });

                this.incidencias = response.data;
                console.log(this.incidencias);
                this.filterIncidencias();
            } catch (error) {
                console.error(error);
            }
        },
        filterIncidencias() {
            this.filteredIncidencias = this.incidencias.filter(incidencia => {
                return incidencia.horaEntrada.substring(0, 10) == this.selectedDate;
            });
        },
        formatDate(dateString) {
            const date = new Date(dateString);
            return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
        },
        editIncidencia(incidencia) {
            this.$router.push({name: 'editar-incidencia', query: {incidencia: incidencia.id}});
        },
        async eliminarElemento(id) {
            try {
                const response = await axios.delete(`https://${ip_address}:7207/api/Incidencia/${id}`, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                });

                console.log("Incidencia eliminada:", response.data);
                this.fetchIncidencias();
            } catch (error) {
                console.error('Error deleting element:', error);
            }
        },
        checkAdmin() {
            const tipoUsuario = localStorage.getItem('user_type');

            if (tipoUsuario == 1) {
                this.isAdmin = true;
            } else {
                this.$router.push('/pantalla-usuario');
            }
        },
        logout() {
            localStorage.removeItem('token');
            window.location.href = '/login-component';
        },
        exportToPDF() {
            const doc = new jsPDF('p', 'pt', 'a4');
            const title = "Tabla de ingresos";
            const margin = 40;
            const accionesIndex = 5;
            const currentDate = new Date();
            const formattedDate = currentDate.toLocaleDateString();
            const formattedTime = currentDate.toLocaleTimeString();
            const calendarDate = document.getElementById('calendario').value;

            doc.setFontSize(18);
            doc.text(title, margin, 40);

            doc.setFontSize(10);
            doc.text(`Fecha de creación: ${formattedDate} ${formattedTime}`, margin, 60);
            doc.text(`Fecha de los ingresos: ${calendarDate}`, margin, 80);
            doc.text(`Creado por: ${localStorage.getItem('name')}`, margin, 100);

            const element = document.getElementById("table-export");

            if (!element) {
                console.error("No se encontró el elemento con ID table-export");
                return;
            }

            const headers = Array.from(element.querySelectorAll('th')).map(th => th.innerText)
                .filter((_, index) => index != accionesIndex);
            const rows = Array.from(element.querySelectorAll('tbody tr')).map(row => {
                return Array.from(row.querySelectorAll('td')).map(td => td.innerText);
            }).map(row => {
                return row.filter((_, index) => index != accionesIndex);
            });

            doc.autoTable({
                head: [headers],
                body: rows,
                startY: 110,
                margin: { left: 40 },
                theme: 'grid',
                headStyles: { fillColor: [22, 160, 133] },
                styles: {
                    fontSize: 10,
                    cellPadding: 5
                },
                didDrawPage: (data) => {
                    const pageSize = doc.internal.pageSize;
                    const pageHeight = pageSize.height ? pageSize.height : pageSize.getHeight();
                    doc.text(`${doc.internal.getNumberOfPages()}`, data.settings.margin.left, pageHeight - 10);
                }
            });

            doc.save(`Tabla_${formattedDate}_${formattedTime}.pdf`);
        },
        goToIncidencia() {
            this.$router.push('/registrar-incidencia');
        },
        goToList() {
            this.$router.push('/lista-usuarios')
        },
        goToEdit() {
            this.$router.push('/editar-incidencia')
        },
        goToContrasena() {
            this.$router.push('/cambiar-contrasena');
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

input {
    padding: 10px;
    border-radius: 5px;
}

button:hover {
    cursor: pointer;
}

.comm {
    max-width: 300px;
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