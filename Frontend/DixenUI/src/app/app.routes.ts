import { Routes } from '@angular/router';
import { CategoryComponent } from './Admin/category-component/category-component';
import { Home } from './components/home/home';
import { Unauthorized } from './components/unauthorized/unauthorized';
import { EventComponent } from './Admin/event-component/event-component';
import { VenueComponent } from './Admin/venue-component/venue-component';
import { HallComponent } from './Admin/hall-component/hall-component';
import { BookingComponent } from './Admin/booking-component/booking-component';
import { CategoryDetail } from './Detail/category-detail/category-detail';
import { EventDetailComponent } from './Detail/event-detail-component/event-detail-component';
import { PerformerComponent } from './Admin/performer-component/performer-component';
import { TicketComponent } from './Admin/ticket-component/ticket-component';
import { Analysis2 } from './DataAnalysis/analysis2/analysis2';
import { RecommendedEvent } from './components/recommended-event/recommended-event';
import { authGuard } from './auth/auth-guard';
import { Userprofile } from './components/userprofile/userprofile';
import { Search } from './Filter/search/search';
import { Analysis } from './DataAnalysis/analysis/analysis';
import { AdminSubmissionsComponent } from './Admin/admin-submissions-component/admin-submissions-component';
import { SubmitEventComponent } from './components/submit-event-component/submit-event-component';

export const routes: Routes = [
  { path: '', component: Home },

  {
    path: 'login',
    loadComponent: () => import('./Account/login/login').then((m) => m.Login),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./Account/registration/registration').then((m) => m.Registration),
  },
  {
    path: 'verify',
    loadComponent: () =>
      import('./Account/verify2fa/verify2fa').then((m) => m.Verify2fa),
  },
  {
    path: 'conform-email',
    loadComponent: () =>
      import('./Account/conform-email/conform-email').then(
        (m) => m.ConformEmail,
      ),
  },

  {
    path: 'user-dashboard',
    loadComponent: () =>
      import('./Dashboards/user-dashboard/user-dashboard').then(
        (m) => m.UserDashboard,
        
      ),
  },
 {
  path: 'admin-dashboard',
  loadComponent: () => import('./Dashboards/admin-dashboard/admin-dashboard').then(m => m.AdminDashboard),
  canActivate: [authGuard],
  data: { roles: ['Admin'] }, 
},

  {
    path: 'host-dashboard',
    loadComponent: () =>
      import('./Dashboards/host-dashboard/host-dashboard').then(
        (m) => m.HostDashboard,
      ),
  },

  { path: 'user-profile/:email', component: Userprofile },

  {
    path: 'admin/category',
    component: CategoryComponent,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'admin/event-submissions',
    component: AdminSubmissionsComponent,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },

  {
    path: 'admin/event',
    component: EventComponent,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'admin/venue',
    component: VenueComponent,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'admin/hall',
    component: HallComponent,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'admin/booking',
    component: BookingComponent,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'admin/performer',
    component: PerformerComponent,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },// need to add host as well
  },

  {
    path: 'admin/ticket',
    component: TicketComponent,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },

  // PUBLIC DETAIL PAGES
  {path: 'submit', component:SubmitEventComponent},
  { path: 'filter/search', component: Search },
  { path: 'recommended', component: RecommendedEvent },
  { path: 'category/:categoryId', component: CategoryDetail },
  { path: 'event/:id', component: EventDetailComponent },
  { path: 'dataanalysis/analysis2', component: Analysis2 },
  { path: 'dataanalysis/analysis', component: Analysis },
  { path: 'unauthorized', component: Unauthorized },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
