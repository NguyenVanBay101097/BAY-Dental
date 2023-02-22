import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TeethChartComponent } from './teeth-chart.component';

describe('TeethChartComponent', () => {
  let component: TeethChartComponent;
  let fixture: ComponentFixture<TeethChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TeethChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TeethChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
