import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AiFacialDetailsComponent } from './ai-facial-details.component';

describe('AiFacialDetailsComponent', () => {
  let component: AiFacialDetailsComponent;
  let fixture: ComponentFixture<AiFacialDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AiFacialDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AiFacialDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
