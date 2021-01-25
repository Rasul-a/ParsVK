import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../profile.service';
import { ProfileResponse } from '../ProfileResponse';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private http: HttpClient, private profileService: ProfileService) {

  }
  title = 'ClientApp';
  response: any;
  allProfiles: any;
  profile: ProfileResponse=null;
  link: string="";
  isAccess: boolean=true;
  load: boolean=false;
  showWall: boolean=false;
  showLikeUsers: boolean=false;
  sortArr: any;
  error: string = null;
  loadProfile: boolean;

  ngOnInit() : void{
    this.getAll()
  }

  toggle(c: string){
    if (c=="wall"){
      this.showWall = !this.showWall;
      this.showLikeUsers = false;
    }
    if (c=="like"){
      this.showLikeUsers = !this.showLikeUsers;
      this.showWall = false;
    }
    
  }

  getAll(){
    this.profileService.getAll().subscribe(data=>this.allProfiles=data);
  }

  parseProfile(){
    if (this.link.trim()=='')
      return
    this.load=true;
    this.error=null;
    this.profileService.parseProfile(this.link).subscribe(
      data=>{
        this.getAll();
        this.response=data;
        this.load=false;
      },
      data=>{
        console.log("error:",data.error)
        this.load=false;
        if (data.error=="5 User authorization failed: access_token has expired.")
          this.isAccess=false;
        else
          this.error=data.error;
    });
  }

  getDetails(id: any){
    this.loadProfile=true;
    this.profileService.getDetails(id).subscribe(
      (data: ProfileResponse)=>
      {
        this.profile=data;
        this.loadProfile=false;
      });

  }
  deleteProfile(id: any){
    this.profileService.delete(id).subscribe(data=>this.getAll());
  }

  sortWall(filed: string){
    if (filed=="like")
      this.sortArr = this.profile.WallItems.sort((a,b)=>(a.LikesCount>b.LikesCount ? -1:1));
    if (filed=="coment")
      this.sortArr = this.profile.WallItems.sort((a,b)=>(a.CommentsCount>b.CommentsCount ? -1:1));  
    this.profile.WallItems = this.sortArr;
  }

}
