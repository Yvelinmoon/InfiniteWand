# Marcus_Yvelin
# Temp : 2024/4/20 23:02

import pygame
import numpy as np
import sys
import time
import csv

def scale_points(points, target_size=(100, 100)):
    """Scale the points of a drawing to a fixed bounding box size."""
    points_array = np.array(points, dtype=float)
    min_x, min_y = np.min(points_array, axis=0)
    max_x, max_y = np.max(points_array, axis=0)
    if max_x == min_x or max_y == min_y:
        return points_array
    width_scale = target_size[0] / (max_x - min_x)
    height_scale = target_size[1] / (max_y - min_y)
    points_array[:, 0] = (points_array[:, 0] - min_x) * width_scale
    points_array[:, 1] = (points_array[:, 1] - min_y) * height_scale
    return points_array

def calculate_angle_changes(points):
    """Calculate the angle changes between consecutive points in a 2D space."""
    vectors = np.diff(points, axis=0)
    angles = np.arctan2(vectors[:, 1], vectors[:, 0])
    angle_changes = np.diff(angles)
    return angle_changes

def calculate_velocities(points, times):
    distances = np.sqrt(np.sum(np.diff(points, axis=0)**2, axis=1))
    time_deltas = np.diff(times)
    time_deltas = np.where(time_deltas == 0, 1e-10, time_deltas)
    velocities = distances / time_deltas
    return velocities

def calculate_accelerations(velocities, times):
    velocity_changes = np.diff(velocities)
    time_deltas = np.diff(times[1:])
    time_deltas = np.where(time_deltas == 0, 1e-10, time_deltas)
    accelerations = velocity_changes / time_deltas
    return accelerations

def write_to_csv(scaled_points, times, angle_changes, velocities, accelerations):
    with open('mouse_movement_data.csv', 'a', newline='') as csvfile:
        writer = csv.writer(csvfile)
        for i, point in enumerate(scaled_points):
            angle_change = angle_changes[i] if i < len(angle_changes) else None
            velocity = velocities[i] if i < len(velocities) else None
            acceleration = accelerations[i] if i < len(accelerations) else None
            writer.writerow([times[i]] + list(point) + [angle_change, velocity, acceleration])

def handle_drawing(screen, points):
    for point in points:
        pygame.draw.circle(screen, (255, 0, 0), point, 2)

def display_counter(screen, font, count):
    count_text = font.render(f"Actions: {count}", True, (0, 0, 0))
    screen.blit(count_text, (10, 10))

def main():
    pygame.init()
    screen = pygame.display.set_mode((800, 600))
    clock = pygame.time.Clock()
    font = pygame.font.Font(None, 36)

    points = []
    times = []
    target_points = 50
    action_count = 0
    is_drawing = False

    # Initialize the CSV file
    with open('mouse_movement_data.csv', 'w', newline='') as csvfile:
        writer = csv.writer(csvfile)
        writer.writerow(['Timestamp', 'X', 'Y', 'Angle Change', 'Velocity', 'Acceleration'])

    while True:
        screen.fill((255, 255, 255))
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                pygame.quit()
                sys.exit()
            elif event.type == pygame.MOUSEBUTTONDOWN:
                is_drawing = True
                points = []
                times = []
            elif event.type == pygame.MOUSEBUTTONUP:
                is_drawing = False
                if len(points) == target_points:
                    scaled_points = scale_points(points)
                    angle_changes = calculate_angle_changes(scaled_points)
                    velocities = calculate_velocities(scaled_points, times)
                    accelerations = calculate_accelerations(velocities, times)
                    write_to_csv(scaled_points.tolist(), times, angle_changes.tolist(), velocities.tolist(), accelerations.tolist())
                    action_count += 1
            elif event.type == pygame.MOUSEMOTION and is_drawing:
                if len(points) < target_points:
                    points.append(event.pos)
                    times.append(time.perf_counter())

        handle_drawing(screen, points)
        display_counter(screen, font, action_count)

        pygame.display.flip()
        clock.tick(60)

if __name__ == "__main__":
    main()