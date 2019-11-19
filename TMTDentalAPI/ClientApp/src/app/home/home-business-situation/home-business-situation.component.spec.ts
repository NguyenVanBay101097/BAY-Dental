import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeBusinessSituationComponent } from './home-business-situation.component';

describe('HomeBusinessSituationComponent', () => {
  let component: HomeBusinessSituationComponent;
  let fixture: ComponentFixture<HomeBusinessSituationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HomeBusinessSituationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HomeBusinessSituationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
