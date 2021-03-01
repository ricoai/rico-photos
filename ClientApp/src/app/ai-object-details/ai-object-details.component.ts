import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-ai-object-details',
  templateUrl: './ai-object-details.component.html',
  styleUrls: ['./ai-object-details.component.css']
})
export class AiObjectDetailsComponent implements OnInit {

  @Input() objects: any;

  constructor() { }

  ngOnInit() {
  }

}
