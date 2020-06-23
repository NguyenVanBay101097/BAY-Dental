import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AudienceFilterComponent } from './audience-filter.component';

describe('AudienceFilterComponent', () => {
  let component: AudienceFilterComponent;
  let fixture: ComponentFixture<AudienceFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AudienceFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AudienceFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
