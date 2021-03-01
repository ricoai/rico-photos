import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-ai-facial-details',
  templateUrl: './ai-facial-details.component.html',
  styleUrls: ['./ai-facial-details.component.css']
})
export class AiFacialDetailsComponent implements OnInit {

  @Input() aiFacial: any;
  @Input() s3Path: string;

  constructor() { }

  ngOnInit() {

  }

}
