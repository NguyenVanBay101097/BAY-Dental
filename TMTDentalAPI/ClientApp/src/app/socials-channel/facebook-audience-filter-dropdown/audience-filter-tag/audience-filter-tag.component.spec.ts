import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AudienceFilterTagComponent } from './audience-filter-tag.component';

describe('AudienceFilterTagComponent', () => {
  let component: AudienceFilterTagComponent;
  let fixture: ComponentFixture<AudienceFilterTagComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AudienceFilterTagComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AudienceFilterTagComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
