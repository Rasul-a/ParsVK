import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import {Routes, RouterModule} from '@angular/router';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import {ProfileService} from './profile.service';
import { TokenComponent } from './token/token.component';
import { WallItemComponent } from './wall-item/wall-item.component';
import { HomeComponent } from './home/home.component'

const appRoutes: Routes =[
  { path: '', component: HomeComponent},
  { path: 'gettoken', component: TokenComponent}
];

@NgModule({
  declarations: [
    AppComponent,
    TokenComponent,
    WallItemComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forRoot(appRoutes)
  ],
  providers: [ProfileService],
  bootstrap: [AppComponent]
})
export class AppModule { }
