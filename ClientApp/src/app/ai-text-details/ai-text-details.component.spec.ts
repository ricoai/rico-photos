import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AiTextDetailsComponent } from './ai-text-details.component';

describe('AiTextDetailsComponent', () => {
  let component: AiTextDetailsComponent;
  let fixture: ComponentFixture<AiTextDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AiTextDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AiTextDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
