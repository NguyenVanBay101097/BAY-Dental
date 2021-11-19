import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrJobListComponent } from './hr-job-list.component';

describe('HrJobListComponent', () => {
  let component: HrJobListComponent;
  let fixture: ComponentFixture<HrJobListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrJobListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrJobListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
