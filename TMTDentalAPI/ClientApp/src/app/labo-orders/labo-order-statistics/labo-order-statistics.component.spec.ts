import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderStatisticsComponent } from './labo-order-statistics.component';

describe('LaboOrderStatisticsComponent', () => {
  let component: LaboOrderStatisticsComponent;
  let fixture: ComponentFixture<LaboOrderStatisticsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderStatisticsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderStatisticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
