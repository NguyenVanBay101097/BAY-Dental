import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookDashboardComponent } from './facebook-dashboard.component';

describe('FacebookDashboardComponent', () => {
  let component: FacebookDashboardComponent;
  let fixture: ComponentFixture<FacebookDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
