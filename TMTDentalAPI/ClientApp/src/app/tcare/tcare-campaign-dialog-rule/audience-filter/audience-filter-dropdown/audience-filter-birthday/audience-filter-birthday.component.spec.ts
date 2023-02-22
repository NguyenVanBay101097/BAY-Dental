import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AudienceFilterBirthdayComponent } from './audience-filter-birthday.component';

describe('AudienceFilterBirthdayComponent', () => {
  let component: AudienceFilterBirthdayComponent;
  let fixture: ComponentFixture<AudienceFilterBirthdayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AudienceFilterBirthdayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AudienceFilterBirthdayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
