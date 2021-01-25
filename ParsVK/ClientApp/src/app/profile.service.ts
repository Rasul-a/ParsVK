import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {map} from "rxjs/operators";
import { ProfileResponse } from './ProfileResponse';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get("api/getAll");
  }

  parseProfile(link: string){
    return this.http.get("api/parseprofile?link="+link);
  }

  getDetails(id: any){
    return this.http.get("api/getProfile?id="+id).pipe(
      map((data: ProfileResponse)=>{
        data.LikeUsers=data.LikeUsers.sort((a,b)=>(a.LikeCount>b.LikeCount ? -1:1));
        return data;
      }
        
      )
    );
  }

  delete(id: any){
    return this.http.get("api/delete?id="+id);
  }
  getToken(code: string){
    return this.http.get("api/gettoken?code="+code);
  }
}
