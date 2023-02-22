import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AudienceFilterInputComponent } from './audience-filter-input.component';

describe('AudienceFilterInputComponent', () => {
  let component: AudienceFilterInputComponent;
  let fixture: ComponentFixture<AudienceFilterInputComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AudienceFilterInputComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AudienceFilterInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
