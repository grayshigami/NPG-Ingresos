import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/login-component',
            name: 'login-component',
            component: () => import("@/components/login.vue")
        },
        {
            path: '/',
            redirect: 'login-component'
        },
        {
            path: '/registrar-incidencia',
            name: 'registrar-incidencia',
            component: () => import("@/components/registrar-incidencia.vue"),
            props: true,
            meta: { requiresAuth: true }
        },
        {
            path: '/crear-usuario',
            name: 'crear-usuario',
            component: () => import("@/components/crear-usuario.vue"),
            meta: { requiresAuth: true }
        },
        {
            path: '/editar-usuario',
            name: 'editar-usuario',
            component: () => import("@/components/editar-usuario.vue"),
            meta: { requiresAuth: true }
        },
        {
            path: '/pantalla-usuario',
            name: 'pantalla-usuario',
            component: () => import("@/components/pantalla-usuario.vue"),
            meta: { requiresAuth: true, user_type: 0 }
        },
        {
            path: '/pantalla-administrador',
            name: 'pantalla-administrador',
            component: () => import("@/components/pantalla-administrador.vue"),
            meta: { requiresAuth: true, user_type: 1 }
        },
        {
            path: '/lista-usuarios',
            name: 'lista-usuarios',
            component: () => import("@/components/lista-usuarios.vue"),
            meta: { requiresAuth: true }
        },
        {
            path: '/editar-incidencia',
            name: 'editar-incidencia',
            component: () => import("@/components/editar-incidencia.vue"),
            props: true,
            meta: { requiresAuth: true }
        },
        {
            path: '/cambiar-contrasena',
            name: 'cambiar-contrasena',
            component: () => import("@/components/cambiar-contrasena.vue"),
            meta: { requiresAuth: true }
        }
    ]
})

router.beforeEach((to, from, next) => {
    const isAuthenticated = !!localStorage.getItem('token');

    if (to.matched.some(record => record.meta.requiresAuth)) {
        if (!isAuthenticated) {
            next({ name: 'login-component' });
        } else {
            next();
        }
    } else {
        next();
    }
});

export default router
