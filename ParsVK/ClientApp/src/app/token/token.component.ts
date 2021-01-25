import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProfileService } from '../profile.service';

@Component({
  selector: 'app-token',
  template: `
    <p>
      Loading...
    </p>
  `,
  styleUrls: ['./token.component.css']
})
export class TokenComponent implements OnInit {
  code: string;
  constructor(private router: Router, private profileService: ProfileService,private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(c=>this.code=c['code']);
    this.profileService.getToken(this.code).subscribe(d=>{
      this.router.navigate(['/'])
    })
    
  }

}
