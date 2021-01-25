import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {map} from "rxjs/operators";
import { ProfileResponse } from './ProfileResponse';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  url: string="api/Profile/";
  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get(this.url);
  }

  parseProfile(link: string){
    return this.http.get(this.url+"ParseProfile?link="+link );
  }

  getDetails(id: any){
    return this.http.get(this.url+id).pipe(
      map((data: ProfileResponse)=>{
        data.LikeUsers=data.LikeUsers.sort((a,b)=>(a.LikeCount>b.LikeCount ? -1:1));
        return data;
      })
    );
  }

  delete(id: any){
    return this.http.delete(this.url+id);
  }
  getToken(code: string){
    return this.http.get(this.url+"GetToken?code="+code);
  }
}
