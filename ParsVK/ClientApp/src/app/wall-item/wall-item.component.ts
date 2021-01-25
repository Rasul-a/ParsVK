import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-wall-item',
  templateUrl: './wall-item.component.html',
  styleUrls: ['./wall-item.component.css']
})
export class WallItemComponent implements OnInit {

  @Input() item: any;
  constructor() { }

  ngOnInit(): void {
  }

}
