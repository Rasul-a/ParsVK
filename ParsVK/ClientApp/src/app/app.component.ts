import { Component, OnInit } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import {ProfileResponse} from './ProfileResponse';
import { ProfileService } from './profile.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent  {

}
